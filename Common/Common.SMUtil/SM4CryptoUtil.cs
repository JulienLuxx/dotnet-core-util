using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Text;

namespace Common.SMUtil
{
    /// <summary>
    /// SM4 crypto util class
    /// </summary>
    public class SM4CryptoUtil
    {
        private string secretKey = "";

        private string iv = "";

        private bool hexString;

        public SM4CryptoUtil() { }

        /// <summary>
        /// Ctor with key and iv and isHexString
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="iv">iv</param>
        /// <param name="hexString">IsHexString</param>
        public SM4CryptoUtil(string secretKey, string iv, bool hexString = false)
        {
            this.secretKey = secretKey;
            this.iv = iv;
            this.hexString = hexString;
        }

        public void SetIv(string iv)
        {
            this.iv = iv;
        }

        public void SetSecretKey(string secretKey)
        {
            this.secretKey = secretKey;
        }

        public void SetHexString(bool hexString)
        {
            this.hexString = hexString;
        }

        /// <summary>
        ///  Using ECB mode to encrypt plain text
        ///  <para>使用 ECB 模式加密明文，返回十六进制密文</para>
        /// </summary>
        /// <param name="plainText">PlainText / 待加密的明文字符串</param>
        /// <param name="encoding">TextEncoding / 文本编码，解密时需使用相同编码</param>
        /// <returns>HexCipherText / 十六进制密文字符串</returns>
        public string EncryptECB(string plainText, Encoding encoding)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText), "明文不能为空 / Plain text cannot be null");
            }
            Sm4Context context = new Sm4Context();
            context.isPadding = true;
            context.mode = SM4.SM4_ENCRYPT;
            byte[] key = ((!hexString) ? encoding.GetBytes(secretKey) : ByteUtils.GetBytesByHexString(secretKey));
            SM4 sm4 = new SM4();
            sm4.sm4_setkey_enc(context, key);
            byte[] data = sm4.sm4_crypt_ecb(context, encoding.GetBytes(plainText));
            return ByteUtils.ToHexString(data);
        }

        /// <summary>
        ///  Using ECB mode to decrypt cipher text
        ///  <para>使用 ECB 模式解密十六进制密文，与 EncryptECB 输出格式对称</para>
        /// </summary>
        /// <param name="cipherText">HexCipherText / 十六进制密文字符串（由 EncryptECB 生成）</param>
        /// <param name="encoding">TextEncoding / 明文字符编码，需与加密时使用的编码一致</param>
        /// <returns>PlainText / 解密后的明文字符串</returns>
        public string DecryptECB(string cipherText, Encoding encoding)
        {
            if (cipherText == null)
            {
                throw new ArgumentNullException(nameof(cipherText), "密文不能为空 / Cipher text cannot be null");
            }
            if (string.IsNullOrWhiteSpace(cipherText))
            {
                throw new ArgumentException("密文不能为空串或纯空白字符 / Cipher text cannot be empty or whitespace", nameof(cipherText));
            }
            Sm4Context context = new Sm4Context();
            context.isPadding = true;
            context.mode = SM4.SM4_DECRYPT;
            byte[] key = ((!hexString) ? encoding.GetBytes(secretKey) : ByteUtils.GetBytesByHexString(secretKey));
            SM4 sm4 = new SM4();
            sm4.sm4_setkey_dec(context, key);
            byte[] bytes = sm4.sm4_crypt_ecb(context, ByteUtils.GetBytesByHexString(cipherText));
            return encoding.GetString(bytes);
        }

        /// <summary>
        ///  Using CBC mode to encrypt plain text
        ///  <para>使用 CBC 模式加密明文，返回十六进制密文；IV 取自实例配置，其编码后必须为 16 字节</para>
        /// </summary>
        /// <param name="plainText">PlainText / 待加密的明文字符串</param>
        /// <param name="encoding">TextEncoding / 文本编码，解密时需使用相同编码</param>
        /// <returns>HexCipherText / 十六进制密文字符串</returns>
        public string EncryptCBC(string plainText, Encoding encoding)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText), "明文不能为空 / Plain text cannot be null");
            }
            Sm4Context context = new Sm4Context();
            context.isPadding = true;
            context.mode = SM4.SM4_ENCRYPT;
            byte[] key;
            byte[] array;
            if (hexString)
            {
                key = ByteUtils.GetBytesByHexString(secretKey);
                array = ByteUtils.GetBytesByHexString(iv);
            }
            else
            {
                key = encoding.GetBytes(secretKey);
                array = encoding.GetBytes(iv);
            }

            SM4 sm4 = new SM4();
            sm4.sm4_setkey_enc(context, key);
            byte[] data = sm4.sm4_crypt_cbc(context, array, encoding.GetBytes(plainText));
            return ByteUtils.ToHexString(data);
        }

        /// <summary>
        ///  Using CBC mode to decrypt cipher text
        ///  <para>使用 CBC 模式解密十六进制密文，与 EncryptCBC 输出格式对称</para>
        /// </summary>
        /// <param name="cipherText">HexCipherText / 十六进制密文字符串（由 EncryptCBC 生成）</param>
        /// <param name="encoding">TextEncoding / 明文字符编码，需与加密时使用的编码一致</param>
        /// <returns>PlainText / 解密后的明文字符串</returns>
        public string DecryptCBC(string cipherText, Encoding encoding)
        {
            if (cipherText == null)
            {
                throw new ArgumentNullException(nameof(cipherText), "密文不能为空 / Cipher text cannot be null");
            }
            if (string.IsNullOrWhiteSpace(cipherText))
            {
                throw new ArgumentException("密文不能为空串或纯空白字符 / Cipher text cannot be empty or whitespace", nameof(cipherText));
            }
            Sm4Context context = new Sm4Context();
            context.isPadding = true;
            context.mode = SM4.SM4_DECRYPT;
            byte[] key;
            byte[] array;
            if (hexString)
            {
                key = ByteUtils.GetBytesByHexString(secretKey);
                array = ByteUtils.GetBytesByHexString(iv);
            }
            else
            {
                key = encoding.GetBytes(secretKey);
                array = encoding.GetBytes(iv);
            }

            SM4 sm4 = new SM4();
            sm4.sm4_setkey_dec(context, key);
            byte[] bytes = sm4.sm4_crypt_cbc(context, array, ByteUtils.GetBytesByHexString(cipherText));
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Using CTR mode to encrypt plain text
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string EncryptCTR(string plainText, Encoding encoding)
        {
            byte[] keyBytes;
            byte[] ivBytes;
            if (hexString)
            {
                keyBytes = ByteUtils.GetBytesByHexString(secretKey);
                ivBytes = ByteUtils.GetBytesByHexString(iv);
            }
            else
            {
                keyBytes = encoding.GetBytes(secretKey);
                ivBytes = encoding.GetBytes(iv);
            }

            var ctrCipher = CipherUtilities.GetCipher("SM4/CTR/NoPadding");
            var sm4Key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
            var keyParamWithIV = new ParametersWithIV(sm4Key, ivBytes);
            ctrCipher.Init(true, keyParamWithIV);
            var blockSize = ctrCipher.GetBlockSize();
            var plainTextBytes = encoding.GetBytes(plainText);
            var cipherTextBytes = new byte[ctrCipher.GetOutputSize(plainTextBytes.Length)];
            var processLength =
         ctrCipher.ProcessBytes(plainTextBytes, 0, plainTextBytes.Length, cipherTextBytes, 0);
            var finalLength = ctrCipher.DoFinal(cipherTextBytes, processLength);
            var length = cipherTextBytes.Length - (blockSize - finalLength);
            var finalCipherTextBytes = new byte[length < 0 ? finalLength : length];
            Array.Copy(cipherTextBytes, 0, finalCipherTextBytes, 0, finalCipherTextBytes.Length);
            return Hex.ToHexString(cipherTextBytes, false);
        }

        /// <summary>
        ///  Using CTR mode to decrypt cipher text
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string DecryptCTR(string cipherText, Encoding encoding)
        {
            byte[] keyBytes;
            byte[] ivBytes;
            if (hexString)
            {
                keyBytes = ByteUtils.GetBytesByHexString(secretKey);
                ivBytes = ByteUtils.GetBytesByHexString(iv);
                //keyBytes = Hex.Decode(secretKey);
                //ivBytes = Hex.Decode(iv);
            }
            else
            {
                keyBytes = encoding.GetBytes(secretKey);
                ivBytes = encoding.GetBytes(iv);
            }
            var ctrCipher = CipherUtilities.GetCipher("SM4/CTR/NoPadding");
            var sm4Key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);

            var keyParamWithIV = new ParametersWithIV(sm4Key, ivBytes);
            ctrCipher.Init(false, keyParamWithIV);
            var blockSize = ctrCipher.GetBlockSize();
            var cipherTextBytes = Hex.Decode(cipherText);
            var plainTextBytes = new byte[ctrCipher.GetOutputSize(cipherTextBytes.Length)];
            var processLength =
                ctrCipher.ProcessBytes(cipherTextBytes, 0, cipherTextBytes.Length, plainTextBytes, 0);
            var finalLength = ctrCipher.DoFinal(plainTextBytes, processLength);
            var length = plainTextBytes.Length - (blockSize - finalLength);
            var finalPlainTextBytes = new byte[length < 0 ? finalLength : length];
            Array.Copy(plainTextBytes, 0, finalPlainTextBytes, 0, finalPlainTextBytes.Length);
            return encoding.GetString(plainTextBytes);
        }
    }
}
