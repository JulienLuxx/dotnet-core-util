using Common.SMUtil;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Common.XUnitTest
{
    public class SM3UnitTest
    {
        /// <summary>
        /// GB/T 32905-2016 标准测试向量一：SM3("abc")
        /// </summary>
        private const string AbcDigest = "66c7f0f462eeedd9d1f2d46bdc10e4e24167c4875cf2f7a2297da02b8f4ba8e0";

        /// <summary>
        /// GB/T 32905-2016 标准测试向量二：SM3("abcd" 重复 16 次，64 字节)
        /// </summary>
        private const string AbcdX16Digest = "debe9ff92275b8a138604889c18e5a4d6fdb70e5387e5765293dcba39c0c5732";

        /// <summary>
        /// 空消息摘要测试向量：SM3("")
        /// </summary>
        private const string EmptyDigest = "1ab21d8355cfa17f8e61194831e81a8f22bec8c728fefb747ed035eb5082aa2b";

        /// <summary>
        /// SM3 压缩函数分组长度（字节）
        /// </summary>
        private const int BlockSize = 64;

        /// <summary>
        /// HMAC-SM3 输出长度（字节）
        /// </summary>
        private const int HmacByteLength = 32;

        [Theory]
        [InlineData("Gxrc@666")]
        public void HashStrTest(string rawStr)
        {
            var oldHash = "33346634613139343263343534313332653461666262663430306161643834643639393237373138666238636335613739383664613239346336633265306635";
            Guid salt = Guid.Parse("964D7DBD-9A1A-45A4-A672-5EE4D213E1F5");
            var key = "vhgVmaq7SJfkdo_q";
            var hash = Sm3CryptoUtil.ToSM3String(rawStr+salt.ToString(), key, Encoding.UTF8);

            Assert.Equal(oldHash, hash);
        }

        [Theory]
        [InlineData("Gxrc@666")]
        public void SaltedKeyedHashSelfVerifyTest(string rawStr)
        {
            // arrange：与 HashStrTest 相同输入形态——明文拼接 Guid 盐串，加固定 key 做 HMAC-SM3
            var salt = Guid.Parse("964D7DBD-9A1A-45A4-A672-5EE4D213E1F5");
            var otherSalt = Guid.Parse("11111111-2222-3333-4444-555555555555");
            var key = "vhgVmaq7SJfkdo_q";
            var saltedInput = rawStr + salt.ToString();

            // act：同一输入重复哈希，结果由被测代码自身产出
            var hash1 = SM3Util.Hash(saltedInput, key, Encoding.UTF8);
            var hash2 = SM3Util.Hash(saltedInput, key, Encoding.UTF8);

            // assert：确定性、32 字节摘要单次 hex（64 字符）
            Assert.Equal(hash1, hash2);
            Assert.Equal(64, hash1.Length);

            // 盐敏感性：换盐或不拼盐，哈希均应不同
            Assert.NotEqual(hash1, SM3Util.Hash(rawStr + otherSalt.ToString(), key, Encoding.UTF8));
            Assert.NotEqual(hash1, SM3Util.Hash(rawStr, key, Encoding.UTF8));
        }

        [Fact]
        public void HashAbcStandardVectorTest()
        {
            // act
            var digest = SM3Util.Hash("Gxrc@666", Encoding.UTF8);

            // assert
            Assert.Equal(AbcDigest, digest);
        }

        [Fact]
        public void HashAbcdX16StandardVectorTest()
        {
            // arrange：64 字节消息，恰好一个完整分组，填充后跨块
            var data = string.Concat(Enumerable.Repeat("abcd", 16));

            // act
            var digest = SM3Util.Hash(data, Encoding.UTF8);

            // assert
            Assert.Equal(AbcdX16Digest, digest);
        }

        [Fact]
        public void HashEmptyStringTest()
        {
            // act
            var digest = SM3Util.Hash(string.Empty, Encoding.UTF8);

            // assert
            Assert.Equal(EmptyDigest, digest);
        }

        [Fact]
        public void HashBytesAndStringOverloadConsistencyTest()
        {
            // arrange：含中文等多字节字符
            var data = "国密SM3摘要测试 Hello World";

            // act：字节重载手动转 hex 与字符串重载结果应一致（单次编码）
            var fromBytes = Hex.ToHexString(SM3Util.Hash(Encoding.UTF8.GetBytes(data)));
            var fromString = SM3Util.Hash(data, Encoding.UTF8);

            // assert
            Assert.Equal(fromBytes, fromString);
        }

        [Fact]
        public void HashLengthBoundaryTest()
        {
            // arrange：覆盖 55/56（填充边界）、63/64/65（分组边界）、119/120（双分组填充边界）等关键长度
            var lengths = new[] { 0, 1, 3, 4, 5, 15, 16, 55, 56, 63, 64, 65, 111, 112, 119, 120, 127, 128, 129, 255, 256, 1000 };

            foreach (var length in lengths)
            {
                // ASCII 范围字节保证 UTF-8 编解码可无损往返
                var msg = new byte[length];
                for (var i = 0; i < length; i++)
                {
                    msg[i] = (byte)(i % 128);
                }

                // act
                var digest = SM3Util.Hash(msg);
                var digestHex = SM3Util.Hash(Encoding.UTF8.GetString(msg), Encoding.UTF8);

                // assert：长度恒定、确定性、字节/字符串重载一致
                Assert.Equal(32, digest.Length);
                Assert.Equal(digest, SM3Util.Hash(msg));
                Assert.Equal(Hex.ToHexString(digest), digestHex);
            }
        }

        [Fact]
        public void HmacShortKeyCrossCheckTest()
        {
            // arrange：短密钥（不足一个分组）
            var key = Encoding.UTF8.GetBytes("sm3-hmac-key");
            var data = Encoding.UTF8.GetBytes("abc");

            // act
            var actual = Hex.ToHexString(SM3Util.Hash(data, key));

            // assert：与 RFC 2104 手工构造的 HMAC 一致
            Assert.Equal(ManualHmacSm3(data, key), actual);
        }

        [Fact]
        public void HmacLongKeyCrossCheckTest()
        {
            // arrange：长密钥（超过分组长度 64，走先哈希再补零的密钥归一化分支）
            var key = new byte[200];
            for (var i = 0; i < key.Length; i++)
            {
                key[i] = (byte)(i % 251);
            }
            var data = Encoding.UTF8.GetBytes("long key path");

            // act
            var actual = Hex.ToHexString(SM3Util.Hash(data, key));

            // assert
            Assert.Equal(ManualHmacSm3(data, key), actual);
        }

        [Fact]
        public void HmacEmptyKeyAndDataCrossCheckTest()
        {
            // arrange：空密钥 / 空数据均为 HMAC 合法输入
            var data = Encoding.UTF8.GetBytes("abc");

            // act
            var emptyKeyActual = Hex.ToHexString(SM3Util.Hash(data, new byte[0]));
            var emptyDataActual = Hex.ToHexString(SM3Util.Hash(new byte[0], Encoding.UTF8.GetBytes("k")));

            // assert
            Assert.Equal(ManualHmacSm3(data, new byte[0]), emptyKeyActual);
            Assert.Equal(ManualHmacSm3(new byte[0], Encoding.UTF8.GetBytes("k")), emptyDataActual);
        }

        [Fact]
        public void HmacKeySensitivityAndDeterminismTest()
        {
            // arrange
            var data = "abc";

            // act
            var mac1 = SM3Util.Hash(data, "key1", Encoding.UTF8);
            var mac1Again = SM3Util.Hash(data, "key1", Encoding.UTF8);
            var mac2 = SM3Util.Hash(data, "key2", Encoding.UTF8);
            var plain = SM3Util.Hash(data, Encoding.UTF8);

            // assert：确定性、密钥敏感性、与无密钥摘要不同
            Assert.Equal(mac1, mac1Again);
            Assert.NotEqual(mac1, mac2);
            Assert.NotEqual(plain, mac1);
        }

        [Fact]
        public void HmacStringAndBytesOverloadConsistencyTest()
        {
            // arrange
            var data = "国密SM3摘要测试";
            var key = "secret-key";

            // act
            var fromStrings = SM3Util.Hash(data, key, Encoding.UTF8);
            var fromBytes = Hex.ToHexString(SM3Util.Hash(Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(key)));

            // assert
            Assert.Equal(fromStrings, fromBytes);
        }

        [Fact]
        public void ArgumentNullValidationTest()
        {
            // 标准摘要
            Assert.Throws<ArgumentNullException>(() => SM3Util.Hash((byte[])null));
            Assert.Throws<ArgumentNullException>(() => SM3Util.Hash((string)null, Encoding.UTF8));
            Assert.Throws<ArgumentNullException>(() => SM3Util.Hash("abc", null));

            // HMAC-SM3
            Assert.Throws<ArgumentNullException>(() => SM3Util.Hash((byte[])null, new byte[16]));
            Assert.Throws<ArgumentNullException>(() => SM3Util.Hash(new byte[3], null));
            Assert.Throws<ArgumentNullException>(() => SM3Util.Hash((string)null, "key", Encoding.UTF8));
            Assert.Throws<ArgumentNullException>(() => SM3Util.Hash("abc", null, Encoding.UTF8));
            Assert.Throws<ArgumentNullException>(() => SM3Util.Hash("abc", "key", null));
        }

        /// <summary>
        /// 按 RFC 2104 公式手工构造 HMAC-SM3，用于独立交叉验证 HMac 装配是否正确：
        /// HMAC(K, m) = SM3((K' ^ opad) || SM3((K' ^ ipad) || m))
        /// </summary>
        /// <param name="data">消息</param>
        /// <param name="key">密钥</param>
        /// <returns>小写十六进制 HMAC-SM3</returns>
        private static string ManualHmacSm3(byte[] data, byte[] key)
        {
            // 密钥归一化：超过分组长度先做 SM3，再补零到分组长度
            var k = key.Length > BlockSize ? SM3Util.Hash(key) : key;
            var kPad = new byte[BlockSize];
            Array.Copy(k, 0, kPad, 0, k.Length);

            var inner = new byte[BlockSize + data.Length];
            var outer = new byte[BlockSize + 32];
            for (var i = 0; i < BlockSize; i++)
            {
                inner[i] = (byte)(kPad[i] ^ 0x36);
                outer[i] = (byte)(kPad[i] ^ 0x5C);
            }
            Array.Copy(data, 0, inner, BlockSize, data.Length);

            var innerHash = SM3Util.Hash(inner);
            Array.Copy(innerHash, 0, outer, BlockSize, innerHash.Length);

            return Hex.ToHexString(SM3Util.Hash(outer));
        }

        #region PBKDF2-HMAC-SM3

        [Fact]
        public void Pbkdf2ManualRfc8018CrossCheckTest()
        {
            // 覆盖：1 次迭代边界、单块/多块派生（dkLen > 32 触发第二块）、任意长度截断（dkLen 非整块）
            var cases = new[]
            {
                new { Iterations = 1, DkLen = 32 },
                new { Iterations = 2, DkLen = 32 },
                new { Iterations = 10, DkLen = 16 },
                new { Iterations = 1000, DkLen = 1 },
                new { Iterations = 1000, DkLen = 20 },
                new { Iterations = 1000, DkLen = 32 },
                new { Iterations = 1000, DkLen = 33 },
                new { Iterations = 1000, DkLen = 64 }
            };
            var password = Encoding.UTF8.GetBytes("信创P@ss");
            var salt = Encoding.UTF8.GetBytes("salty-value");

            foreach (var c in cases)
            {
                var expected = ManualPbkdf2(password, salt, c.Iterations, c.DkLen);
                var actual = SM3Util.Pbkdf2(password, salt, c.Iterations, c.DkLen);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Pbkdf2DeterminismAndSensitivityTest()
        {
            // arrange
            var password = Encoding.UTF8.GetBytes("p");
            var salt1 = Encoding.UTF8.GetBytes("salt-1");
            var salt2 = Encoding.UTF8.GetBytes("salt-2");

            // act
            var k1 = SM3Util.Pbkdf2(password, salt1, 100, 32);
            var k1Again = SM3Util.Pbkdf2(password, salt1, 100, 32);
            var k2 = SM3Util.Pbkdf2(password, salt2, 100, 32);
            var k3 = SM3Util.Pbkdf2(password, salt1, 101, 32);
            var sm4Key = SM3Util.Pbkdf2(password, salt1, 100, 16);

            // assert：确定性、盐敏感性、迭代次数敏感性、派生长度可控（16 字节 = SM4 密钥长度）
            Assert.Equal(k1, k1Again);
            Assert.NotEqual(k1, k2);
            Assert.NotEqual(k1, k3);
            Assert.Equal(16, sm4Key.Length);
        }

        [Fact]
        public void HashPasswordAndVerifyRoundTripTest()
        {
            // arrange：含中文与特殊字符的密码，小迭代次数快速往返
            var password = "信创P@ssw0rd!#%";
            var stored = SM3Util.HashPassword(password, iterations: 1000);

            // act
            var verified = SM3Util.VerifyPassword(password, stored);
            var rejected = SM3Util.VerifyPassword("信创P@ssw0rd!# ", stored);

            // assert
            Assert.True(verified);
            Assert.False(rejected);
        }

        [Fact]
        public void HashPasswordEmptyPasswordRoundTripTest()
        {
            // arrange：空密码是 HashPassword 的合法输入（区别于 null）
            var stored = SM3Util.HashPassword(string.Empty, iterations: 100);

            // act & assert
            Assert.True(SM3Util.VerifyPassword(string.Empty, stored));
            Assert.False(SM3Util.VerifyPassword("x", stored));
        }

        [Fact]
        public void HashPasswordDefaultParametersSmokeTest()
        {
            // arrange & act：默认参数（100000 次迭代、16 字节盐、UTF-8 编码）冒烟测试
            var stored = SM3Util.HashPassword("admin");

            // assert：格式前缀、默认迭代次数与默认编码正确，且可成功校验
            Assert.StartsWith("pbkdf2-sm3$100000$utf-8$", stored);
            Assert.True(SM3Util.VerifyPassword("admin", stored));
            Assert.False(SM3Util.VerifyPassword("admin1", stored));
        }

        [Fact]
        public void HashPasswordUniqueSaltTest()
        {
            // arrange & act：同一密码哈希两次
            var stored1 = SM3Util.HashPassword("same-password", iterations: 100);
            var stored2 = SM3Util.HashPassword("same-password", iterations: 100);

            // assert：随机盐保证存储串互不相同，但均可通过校验
            Assert.NotEqual(stored1, stored2);
            Assert.True(SM3Util.VerifyPassword("same-password", stored1));
            Assert.True(SM3Util.VerifyPassword("same-password", stored2));
        }

        [Fact]
        public void HashPasswordCustomSaltLengthTest()
        {
            // arrange & act
            var stored = SM3Util.HashPassword("pwd", iterations: 100, saltLength: 32);

            // assert：盐字段解码后为 32 字节
            var fields = stored.Split('$');
            Assert.Equal(5, fields.Length);
            Assert.Equal(32, Convert.FromBase64String(fields[3]).Length);
            Assert.True(SM3Util.VerifyPassword("pwd", stored));
        }

        [Fact]
        public void HashPasswordNullEncodingDefaultsToUtf8Test()
        {
            // arrange & act：encoding 传 null 应回落到默认 UTF-8
            var stored = SM3Util.HashPassword("pwd", iterations: 100, encoding: null);

            // assert：编码字段为 utf-8 且可正常校验（与不传参数等价）
            Assert.StartsWith("pbkdf2-sm3$100$utf-8$", stored);
            Assert.True(SM3Util.VerifyPassword("pwd", stored));
        }

        [Fact]
        public void HashPasswordUnicodeEncodingRoundTripTest()
        {
            // arrange：UTF-16 为各平台内置编码，无需注册提供程序
            var password = "信创P@ssw0rd";
            var stored = SM3Util.HashPassword(password, iterations: 100, encoding: Encoding.Unicode);

            // assert：编码名入串（utf-16）；校验无需传编码（从串中读回）
            var fields = stored.Split('$');
            Assert.Equal(5, fields.Length);
            Assert.Equal(Encoding.Unicode.WebName, fields[2]);
            Assert.True(SM3Util.VerifyPassword(password, stored));
            Assert.False(SM3Util.VerifyPassword("wrong", stored));
        }

        [Fact]
        public void HashPasswordGbkEncodingRoundTripTest()
        {
            // arrange：GBK 老系统互认场景，需注册 CodePages 编码提供程序
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var gbk = Encoding.GetEncoding("GBK");
            var password = "信创P@ssw0rd";

            // act：按 GBK 编码哈希
            var storedGbk = SM3Util.HashPassword(password, iterations: 100, encoding: gbk);

            // assert：编码字段记录 GBK 的规范名（WebName），校验按串中编码名自动读回
            var fields = storedGbk.Split('$');
            Assert.Equal(5, fields.Length);
            Assert.Equal(gbk.WebName, fields[2]);
            Assert.True(SM3Util.VerifyPassword(password, storedGbk));

            // 篡改编码字段为 utf-8：中文在 GBK/UTF-8 下字节不同 → 派生结果不同 → 校验失败，
            // 证明编码确实参与派生且自描述格式防住了错配
            var tampered = string.Join("$", fields[0], fields[1], "utf-8", fields[3], fields[4]);
            Assert.False(SM3Util.VerifyPassword(password, tampered));
        }

        [Fact]
        public void VerifyPasswordMalformedStoredHashTest()
        {
            // 各类损坏的存储串一律返回 false，不抛异常
            Assert.False(SM3Util.VerifyPassword("x", ""));
            Assert.False(SM3Util.VerifyPassword("x", "   "));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3"));
            // 旧版 4 字段格式（无编码字段）应被拒绝
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$1000$AAAA$AAAA"));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sha256$1000$utf-8$AAAA$AAAA"));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$1000$utf-8$AAAA$AAAA$extra"));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$abc$utf-8$AAAA$AAAA"));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$0$utf-8$AAAA$AAAA"));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$-1$utf-8$AAAA$AAAA"));
            // 空编码名 / 未知编码名
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$1000$$AAAA$AAAA"));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$1000$no-such-encoding$AAAA$AAAA"));
            // 空盐 / 空哈希 / 坏 Base64
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$1000$utf-8$$AAAA"));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$1000$utf-8$AAAA$"));
            Assert.False(SM3Util.VerifyPassword("x", "pbkdf2-sm3$1000$utf-8$!!!$AAAA"));
        }

        [Fact]
        public void VerifyPasswordValidFormatWrongContentTest()
        {
            // arrange：格式合法但哈希内容不匹配
            var stored = SM3Util.HashPassword("correct", iterations: 100);
            var tampered = stored.Substring(0, stored.Length - 4) + "AAAA";

            // act & assert：篡改哈希后缀应校验失败
            Assert.False(SM3Util.VerifyPassword("correct", tampered));
        }

        [Fact]
        public void Pbkdf2ArgumentValidationTest()
        {
            // arrange
            var password = Encoding.UTF8.GetBytes("p");
            var salt = Encoding.UTF8.GetBytes("s");

            // act & assert
            Assert.Throws<ArgumentNullException>(() => SM3Util.Pbkdf2(null, salt, 1, 32));
            Assert.Throws<ArgumentNullException>(() => SM3Util.Pbkdf2(password, null, 1, 32));
            Assert.Throws<ArgumentOutOfRangeException>(() => SM3Util.Pbkdf2(password, salt, 0, 32));
            Assert.Throws<ArgumentOutOfRangeException>(() => SM3Util.Pbkdf2(password, salt, -1, 32));
            Assert.Throws<ArgumentOutOfRangeException>(() => SM3Util.Pbkdf2(password, salt, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => SM3Util.Pbkdf2(password, salt, 1, -1));
        }

        [Fact]
        public void HashPasswordArgumentValidationTest()
        {
            // act & assert
            Assert.Throws<ArgumentNullException>(() => SM3Util.HashPassword(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => SM3Util.HashPassword("p", iterations: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => SM3Util.HashPassword("p", saltLength: 0));
            Assert.Throws<ArgumentNullException>(() => SM3Util.VerifyPassword(null, "pbkdf2-sm3$1$AAAA$AAAA"));
            Assert.Throws<ArgumentNullException>(() => SM3Util.VerifyPassword("p", null));
        }

        /// <summary>
        /// 按 RFC 8018 伪代码手工实现 PBKDF2（PRF = HMAC-SM3），用于独立交叉验证：
        /// T_i = F(P, S, c, i)；F 为 U1 ^ U2 ^ ... ^ Uc；U1 = PRF(P, S || INT_BE32(i))；Uj = PRF(P, U(j-1))
        /// </summary>
        /// <param name="password">口令</param>
        /// <param name="salt">盐</param>
        /// <param name="iterations">迭代次数</param>
        /// <param name="dkLen">派生密钥长度（字节）</param>
        /// <returns>派生密钥</returns>
        private static byte[] ManualPbkdf2(byte[] password, byte[] salt, int iterations, int dkLen)
        {
            var blockCount = (dkLen + HmacByteLength - 1) / HmacByteLength;
            var full = new byte[blockCount * HmacByteLength];
            for (var block = 1; block <= blockCount; block++)
            {
                // U1 = HMAC(P, S || INT(i))，INT(i) 为块序号 i 的 4 字节大端编码
                var input = new byte[salt.Length + 4];
                Array.Copy(salt, 0, input, 0, salt.Length);
                input[salt.Length] = (byte)(block >> 24);
                input[salt.Length + 1] = (byte)(block >> 16);
                input[salt.Length + 2] = (byte)(block >> 8);
                input[salt.Length + 3] = (byte)block;

                var u = SM3Util.Hash(input, password);
                var t = (byte[])u.Clone();
                for (var i = 1; i < iterations; i++)
                {
                    u = SM3Util.Hash(u, password);
                    for (var j = 0; j < HmacByteLength; j++)
                    {
                        t[j] ^= u[j];
                    }
                }
                Array.Copy(t, 0, full, (block - 1) * HmacByteLength, HmacByteLength);
            }

            // 截断到请求长度
            var result = new byte[dkLen];
            Array.Copy(full, 0, result, 0, dkLen);
            return result;
        }

        #endregion
    }
}
