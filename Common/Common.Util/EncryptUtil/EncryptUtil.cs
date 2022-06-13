using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Util
{
    public class EncryptUtil : IEncryptUtil
    {
        #region Md5

        public string GetMd5(string value, Encoding encoding, int? startIncdex, int? length)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            var md5 = new MD5CryptoServiceProvider();
            string result;
            try
            {
                var hash = md5.ComputeHash(encoding.GetBytes(value));
                result = startIncdex == null && length == null ? BitConverter.ToString(hash) : BitConverter.ToString(hash, startIncdex.Value, length.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                md5.Clear();
            }
            return result.Replace("-", "");
        }

        public string GetMd5By16(string value)
        {
            return GetMd5By16(value, Encoding.UTF8);
        }

        public string GetMd5By16(string value, Encoding encoding)
        {
            return GetMd5(value, encoding, 4, 8);
        }

        public string GetMd5By32(string value)
        {
            return GetMd5By32(value, Encoding.UTF8);
        }

        public string GetMd5By32(string value, Encoding encoding)
        {
            return GetMd5(value, encoding, null, null);
        }

        #endregion

        #region SHA1

        public string GetSHA1(string value, Encoding encoding) 
        {
            var valueArray = encoding.GetBytes(value);
            var dataArray = new SHA1CryptoServiceProvider().ComputeHash(valueArray);
            var hash = BitConverter.ToString(dataArray).Replace("-", string.Empty);
            return hash.ToLower();
        }

        #endregion

        #region AES加密

        /// <summary>
        /// 128位0向量
        /// </summary>
        private byte[] _iv;
        /// <summary>
        /// 128位0向量
        /// </summary>
        private byte[] Iv
        {
            get
            {
                if (_iv == null)
                {
                    var size = 16;
                    _iv = new byte[size];
                    for (int i = 0; i < size; i++)
                        _iv[i] = 0;
                }
                return _iv;
            }
        }

        /// <summary>
        /// SetAESIv
        /// </summary>
        /// <param name="IvStr"></param>
        /// <param name="encoding"></param>
        private void SetAesIv(string ivStr,Encoding encoding)
        {
            _iv = encoding.GetBytes(ivStr);
        }

        /// <summary>
        /// AESKey
        /// </summary>
        public string AesKey = "QaP1AF8utIarcBqdhYTZpVGbiNQ9M6IL";

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public string AesEncrypt(string value)
        {
            return AesEncrypt(value, AesKey);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public string AesEncrypt(string value, string key)
        {
            return AesEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public string AesEncrypt(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }                
            var rijndaelManaged = CreateRijndaelManaged(key, encoding);
            using (var transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetEncryptResult(value, encoding, transform);
            }
        }

        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="encoding"></param>
        /// <param name="ivStr"></param>
        /// <returns></returns>
        public string AesEncrypt(string value, string key, Encoding encoding, string ivStr = null)
        {
            if (string.IsNullOrEmpty(ivStr) || string.IsNullOrEmpty(ivStr))
            {
                return AesEncrypt(value, key, encoding);
            }
            else
            {
                var newIv = encoding.GetBytes(ivStr);
                if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                {
                    return string.Empty;
                }
                var rijndaelManaged = CreateRijndaelManaged(key, encoding, newIv);
                using (var transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV))
                {
                    return GetEncryptResult(value, encoding, transform);
                }
            }
        }

        /// <summary>
        /// 创建RijndaelManaged
        /// </summary>
        private RijndaelManaged CreateRijndaelManaged(string key, Encoding encoding, CipherMode cipherMode = CipherMode.CBC)
        {
            return new RijndaelManaged
            {
                Key = encoding.GetBytes(key),
                Mode = cipherMode,
                Padding = PaddingMode.PKCS7,
                IV = Iv
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="encoding"></param>
        /// <param name="iv"></param>
        /// <param name="cipherMode"></param>
        /// <returns></returns>
        private RijndaelManaged CreateRijndaelManaged(string key, Encoding encoding, byte[] iv,CipherMode cipherMode = CipherMode.CBC)
        {
            return new RijndaelManaged
            {
                Key = encoding.GetBytes(key),
                Mode = cipherMode,
                Padding = PaddingMode.PKCS7,
                IV = iv
            };
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public string AesDecrypt(string value)
        {
            return AesDecrypt(value, AesKey);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥</param>
        public string AesDecrypt(string value, string key)
        {
            return AesDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        /// <param name="cipherMode">密码模式</param>
        /// <param name="ivStr"></param>
        public string AesDecrypt(string value, string key, Encoding encoding, CipherMode cipherMode = CipherMode.CBC, string ivStr = null)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(ivStr) || string.IsNullOrEmpty(ivStr))
            {
                var rijndaelManaged = CreateRijndaelManaged(key, encoding, cipherMode);
                using (var transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV))
                {
                    return GetDecryptResult(value, encoding, transform);
                }
            }
            else
            {
                var newIv = encoding.GetBytes(ivStr);
                var rijndaelManaged = CreateRijndaelManaged(key, encoding, newIv, cipherMode);
                using (var transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV))
                {
                    return GetDecryptResult(value, encoding, transform);
                }
            }
        }


        /// <summary>
        /// 获取加密结果
        /// </summary>
        private string GetEncryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = encoding.GetBytes(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Convert.ToBase64String(result);
        }

        /// <summary>
        /// 获取解密结果
        /// </summary>
        private string GetDecryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = System.Convert.FromBase64String(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return encoding.GetString(result);
        }

        #endregion

        #region DES加密

        /// <summary>
        /// DES密钥,24位字符串
        /// </summary>
        public string DesKey = "#s^un2ye21fcv%|f0XpR,+vh";

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public string DesEncrypt(object value)
        {
            return DesEncrypt(value, DesKey);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        public string DesEncrypt(object value, string key)
        {
            return DesEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public string DesEncrypt(object value, string key, Encoding encoding)
        {
            string text = value.SafeString();
            if (ValidateDes(text, key) == false)
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateEncryptor())
            {
                return GetEncryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 验证Des加密参数
        /// </summary>
        private bool ValidateDes(string text, string key)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(key))
                return false;
            return key.Length == 24;
        }

        /// <summary>
        /// 创建Des加密服务提供程序
        /// </summary>
        private static TripleDESCryptoServiceProvider CreateDesProvider(string key)
        {
            return new TripleDESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(key), Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public string DesDecrypt(object value)
        {
            return DesDecrypt(value, DesKey);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        public string DesDecrypt(object value, string key)
        {
            return DesDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public string DesDecrypt(object value, string key, Encoding encoding)
        {
            string text = value.SafeString();
            if (!ValidateDes(text, key))
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateDecryptor())
            {
                return GetDecryptResult(text, encoding, transform);
            }
        }

        #endregion

        #region RSA签名

        /// <summary>
        /// RSA加密，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public string RsaSign(string value, string key)
        {
            return RsaSign(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// RSA加密，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public string RsaSign(string value, string key, Encoding encoding)
        {
            return RsaSign(value, key, encoding, RSAType.RSA);
        }

        /// <summary>
        /// RSA加密，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public string Rsa2Sign(string value, string key)
        {
            return Rsa2Sign(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// RSA加密，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public string Rsa2Sign(string value, string key, Encoding encoding)
        {
            return RsaSign(value, key, encoding, RSAType.RSA2);
        }

        /// <summary>
        /// Rsa加密
        /// </summary>
        private string RsaSign(string value, string key, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rsa = new RsaHelper(type, encoding, key);
            return rsa.Sign(value);
        }

        /// <summary>
        /// Rsa验签，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        public bool RsaVerify(string value, string publicKey, string sign)
        {
            return RsaVerify(value, publicKey, sign, Encoding.UTF8);
        }

        /// <summary>
        /// Rsa验签，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        /// <param name="encoding">编码</param>
        public bool RsaVerify(string value, string publicKey, string sign, Encoding encoding)
        {
            return RsaVerify(value, publicKey, sign, encoding, RSAType.RSA);
        }

        /// <summary>
        /// Rsa验签，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        public bool Rsa2Verify(string value, string publicKey, string sign)
        {
            return Rsa2Verify(value, publicKey, sign, Encoding.UTF8);
        }

        /// <summary>
        /// Rsa验签，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        /// <param name="encoding">编码</param>
        public bool Rsa2Verify(string value, string publicKey, string sign, Encoding encoding)
        {
            return RsaVerify(value, publicKey, sign, encoding, RSAType.RSA2);
        }

        /// <summary>
        /// Rsa验签
        /// </summary>
        private bool RsaVerify(string value, string publicKey, string sign, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(sign))
                return false;
            var rsa = new RsaHelper(type, encoding, publicKey: publicKey);
            return rsa.Verify(value, sign);
        }

        #endregion

        public string CreateRandomCode(int codeLength, bool isPurelyNumerical=true) 
        {
            var chars = string.Empty;
            var sum = 0;
            if (isPurelyNumerical)
            {
                chars = "0,1,2,3,4,5,6,7,8,9";
                sum = 10;
            }
            else 
            {
                chars= "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
                sum = 35;
            }
            var charsArray = chars.Split(',');
            var temp = -1;
            var randomCode = string.Empty;
            var random = new Random();
            for (int i = 0; i < codeLength; i++)
            {
                if (-1 != temp)
                {
                    random = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                var t = random.Next(sum);
                if (temp==t)
                {
                    return CreateRandomCode(codeLength);
                }
                temp = t;
                randomCode += charsArray[t];
            }
            return randomCode;
        }

        public bool GetLongByGuid(out long num)
        {
            try
            {
                byte[] buffer = Guid.NewGuid().ToByteArray();
                num = BitConverter.ToInt64(buffer, 0);
                return true;
            }
            catch (Exception ex)
            {
                num = 0;
                return false;
            }
        }

        public bool GetLongByGuid(Guid guid, out long num)
        {
            if (null == guid || guid == Guid.Empty)
            {
                num = 0;
                return false;
            }
            try
            {
                byte[] buffer = guid.ToByteArray();
                num = BitConverter.ToInt64(buffer, 0);
                return true;
            }
            catch (Exception ex)
            {
                num = 0;
                return false;
            }
        }

        public long GetNowTimestamp(DateTime? date = null)
        {
            date = date.HasValue ? date : DateTime.Now;
            var ts = date.Value.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var lts = (long)ts.TotalSeconds;
            return lts;
        }
    }
}
