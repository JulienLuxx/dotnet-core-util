using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Text;

namespace Common.SMUtil
{
    /// <summary>
    /// SM3 hash utility based on BouncyCastle
    /// 基于 BouncyCastle 的 SM3 哈希工具类
    /// <para>提供标准摘要与带密钥摘要（HMAC-SM3），摘要长度 32 字节，十六进制输出仅做一次编码</para>
    /// <para>并提供 PBKDF2-HMAC-SM3 密钥派生（RFC 8018）与密码存储/校验套件，适用于信创系统口令保护与密钥派生</para>
    /// </summary>
    public static class SM3Util
    {
        /// <summary>
        /// SM3 digest size in bytes
        /// SM3 摘要长度（字节）
        /// </summary>
        private const int DigestByteLength = 32;

        /// <summary>
        /// Algorithm prefix of the password storage format
        /// 密码存储格式的算法前缀
        /// </summary>
        private const string AlgorithmPrefix = "pbkdf2-sm3";

        /// <summary>
        /// Field separator of the password storage format
        /// 密码存储格式的字段分隔符
        /// </summary>
        private const char FieldSeparator = '$';

        /// <summary>
        /// Field count of the password storage format (prefix, iterations, encoding, salt, hash)
        /// 密码存储格式的字段数量（前缀、迭代次数、编码名、盐、哈希）
        /// </summary>
        private const int StoredHashFieldCount = 5;

        /// <summary>
        /// Default PBKDF2 iteration count
        /// PBKDF2 默认迭代次数
        /// </summary>
        private const int DefaultIterations = 100000;

        /// <summary>
        /// Default salt length in bytes
        /// 默认盐长度（字节）
        /// </summary>
        private const int DefaultSaltLength = 16;

        /// <summary>
        /// SM3 standard digest
        /// SM3 标准摘要（不带密钥）
        /// </summary>
        /// <param name="data">原始数据 / Raw data</param>
        /// <returns>32 字节摘要 / 32-byte digest</returns>
        public static byte[] Hash(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "原始数据不能为空 / Data cannot be null");
            }

            var digest = new Org.BouncyCastle.Crypto.Digests.SM3Digest();
            digest.BlockUpdate(data, 0, data.Length);
            var result = new byte[DigestByteLength];
            digest.DoFinal(result, 0);
            return result;
        }

        /// <summary>
        /// SM3 standard digest
        /// SM3 标准摘要（不带密钥）
        /// </summary>
        /// <param name="data">原始数据 / Raw data</param>
        /// <param name="encoding">文本编码 / Text encoding</param>
        /// <returns>小写十六进制摘要（仅一次编码）/ Digest in lowercase hex (encoded once)</returns>
        public static string Hash(string data, Encoding encoding)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "原始数据不能为空 / Data cannot be null");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding), "文本编码不能为空 / Encoding cannot be null");
            }

            return Hex.ToHexString(Hash(encoding.GetBytes(data)));
        }

        /// <summary>
        /// HMAC-SM3 digest (keyed)
        /// HMAC-SM3 带密钥摘要
        /// </summary>
        /// <param name="data">原始数据 / Raw data</param>
        /// <param name="key">密钥 / Key</param>
        /// <returns>32 字节摘要 / 32-byte digest</returns>
        public static byte[] Hash(byte[] data, byte[] key)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "原始数据不能为空 / Data cannot be null");
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "密钥不能为空 / Key cannot be null");
            }

            var mac = new HMac(new Org.BouncyCastle.Crypto.Digests.SM3Digest());
            mac.Init(new KeyParameter(key));
            mac.BlockUpdate(data, 0, data.Length);
            var result = new byte[mac.GetMacSize()];
            mac.DoFinal(result, 0);
            return result;
        }

        /// <summary>
        /// HMAC-SM3 digest (keyed)
        /// HMAC-SM3 带密钥摘要
        /// </summary>
        /// <param name="data">原始数据 / Raw data</param>
        /// <param name="key">密钥 / Key</param>
        /// <param name="encoding">文本编码 / Text encoding</param>
        /// <returns>小写十六进制摘要（仅一次编码）/ Digest in lowercase hex (encoded once)</returns>
        public static string Hash(string data, string key, Encoding encoding)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "原始数据不能为空 / Data cannot be null");
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "密钥不能为空 / Key cannot be null");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding), "文本编码不能为空 / Encoding cannot be null");
            }

            return Hex.ToHexString(Hash(encoding.GetBytes(data), encoding.GetBytes(key)));
        }

        /// <summary>
        /// PBKDF2 key derivation using HMAC-SM3 as the PRF (RFC 8018)
        /// 以 HMAC-SM3 为伪随机函数的 PBKDF2 密钥派生（RFC 8018）
        /// <para>信创场景下用于替代 PBKDF2-HMAC-SHA256：派生 16 字节可直接作为 SM4 密钥</para>
        /// </summary>
        /// <param name="password">派生口令 / Derivation password</param>
        /// <param name="salt">盐 / Salt</param>
        /// <param name="iterations">迭代次数，须为正整数 / Iteration count, must be positive</param>
        /// <param name="derivedKeyLength">派生密钥长度（字节）/ Derived key length in bytes</param>
        /// <returns>派生密钥 / Derived key</returns>
        public static byte[] Pbkdf2(byte[] password, byte[] salt, int iterations, int derivedKeyLength)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password), "口令不能为空 / Password cannot be null");
            }
            if (salt == null)
            {
                throw new ArgumentNullException(nameof(salt), "盐不能为空 / Salt cannot be null");
            }
            if (iterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iterations), "迭代次数必须为正整数 / Iterations must be a positive integer");
            }
            if (derivedKeyLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(derivedKeyLength), "派生密钥长度必须为正整数 / Derived key length must be a positive integer");
            }

            var generator = new Pkcs5S2ParametersGenerator(new Org.BouncyCastle.Crypto.Digests.SM3Digest());
            generator.Init(password, salt, iterations);
            var derivedKey = (KeyParameter)generator.GenerateDerivedMacParameters(derivedKeyLength * 8);
            return derivedKey.GetKey();
        }

        /// <summary>
        /// Hash a password for storage using PBKDF2-HMAC-SM3 with a random salt
        /// 使用 PBKDF2-HMAC-SM3 与随机盐对密码进行哈希，用于存储
        /// <para>输出格式：pbkdf2-sm3$迭代次数$编码名$盐Base64$哈希Base64，可直接入库</para>
        /// <para>编码名写入存储串实现自描述：VerifyPassword 校验时自动读回，无需调用方再传编码</para>
        /// <para>对接 GBK 等老系统时通过 encoding 参数指定；编码名无法解析（如未注册 CodePagesEncodingProvider）时校验返回 false</para>
        /// </summary>
        /// <param name="password">明文密码 / Plain password</param>
        /// <param name="iterations">迭代次数，默认 100000 / Iteration count, default 100000</param>
        /// <param name="saltLength">盐长度（字节），默认 16 / Salt length in bytes, default 16</param>
        /// <param name="encoding">密码编码，null 时默认 UTF-8 / Password encoding, defaults to UTF-8 when null</param>
        /// <returns>格式化存储串 / Formatted string for storage</returns>
        public static string HashPassword(string password, int iterations = DefaultIterations, int saltLength = DefaultSaltLength, Encoding encoding = null)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password), "密码不能为空 / Password cannot be null");
            }
            if (iterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iterations), "迭代次数必须为正整数 / Iterations must be a positive integer");
            }
            if (saltLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(saltLength), "盐长度必须为正整数 / Salt length must be a positive integer");
            }

            var effectiveEncoding = encoding ?? Encoding.UTF8;
            var salt = new byte[saltLength];
            new SecureRandom().NextBytes(salt);
            var derivedKey = Pbkdf2(effectiveEncoding.GetBytes(password), salt, iterations, DigestByteLength);
            return $"{AlgorithmPrefix}{FieldSeparator}{iterations}{FieldSeparator}{effectiveEncoding.WebName}{FieldSeparator}{Convert.ToBase64String(salt)}{FieldSeparator}{Convert.ToBase64String(derivedKey)}";
        }

        /// <summary>
        /// Verify a password against a stored hash produced by <see cref="HashPassword(string, int, int, Encoding)"/>
        /// 校验密码与 HashPassword 生成的存储串是否匹配
        /// <para>密码编码从存储串的编码名字段自动读回，无需调用方传入</para>
        /// <para>采用恒定时间比较，防范时序侧信道攻击；存储串格式无效（含编码名无法解析）时返回 false 而非抛异常</para>
        /// </summary>
        /// <param name="password">待校验的明文密码 / Plain password to verify</param>
        /// <param name="storedHash">HashPassword 生成的存储串 / Stored hash produced by HashPassword</param>
        /// <returns>匹配返回 true，否则 false / true if matched, otherwise false</returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password), "密码不能为空 / Password cannot be null");
            }
            if (storedHash == null)
            {
                throw new ArgumentNullException(nameof(storedHash), "存储串不能为空 / Stored hash cannot be null");
            }

            var fields = storedHash.Split(FieldSeparator);
            if (fields.Length != StoredHashFieldCount || fields[0] != AlgorithmPrefix)
            {
                return false;
            }
            if (!int.TryParse(fields[1], out var iterations) || iterations <= 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(fields[2]))
            {
                return false;
            }

            Encoding encoding;
            try
            {
                encoding = Encoding.GetEncoding(fields[2]);
            }
            catch (ArgumentException)
            {
                // 编码名无法解析（如未注册 CodePagesEncodingProvider 的 GBK），视为格式无效
                return false;
            }

            byte[] salt;
            byte[] expectedKey;
            try
            {
                salt = Convert.FromBase64String(fields[3]);
                expectedKey = Convert.FromBase64String(fields[4]);
            }
            catch (FormatException)
            {
                // Base64 字段损坏，视为格式无效
                return false;
            }
            if (salt.Length == 0 || expectedKey.Length == 0)
            {
                return false;
            }

            var actualKey = Pbkdf2(encoding.GetBytes(password), salt, iterations, expectedKey.Length);
            return FixedTimeEquals(actualKey, expectedKey);
        }

        /// <summary>
        /// Constant-time byte array equality check
        /// 恒定时间字节数组相等比较
        /// </summary>
        /// <param name="left">左操作数 / Left operand</param>
        /// <param name="right">右操作数 / Right operand</param>
        /// <returns>长度相等且逐字节相同返回 true / true if same length and identical bytes</returns>
        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }

            var diff = 0;
            for (var i = 0; i < left.Length; i++)
            {
                diff |= left[i] ^ right[i];
            }
            return diff == 0;
        }
    }
}
