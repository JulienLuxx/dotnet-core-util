using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Util
{
    public interface IEncryptUtil : IDependency
    {
        #region md5

        string GetMd5By16(string value);

        string GetMd5By16(string value, Encoding encoding);

        string GetMd5By32(string value);

        string GetMd5By32(string value, Encoding encoding);

        #endregion       

        #region AES

        string AesEncrypt(string value);

        string AesEncrypt(string value, string key);

        string AesEncrypt(string value, string key, Encoding encoding, string ivStr = null);

        string AesDecrypt(string value);

        string AesDecrypt(string value, string key);

        string AesDecrypt(string value, string key, Encoding encoding, CipherMode cipherMode = CipherMode.CBC, string ivStr = null);

        #endregion

        #region DES

        string DesEncrypt(object value);

        string DesEncrypt(object value, string key);

        string DesDecrypt(object value);

        string DesDecrypt(object value, string key);

        #endregion

        #region RSA

        string RsaSign(string value, string key);

        string RsaSign(string value, string key, Encoding encoding);

        string Rsa2Sign(string value, string key);

        string Rsa2Sign(string value, string key, Encoding encoding);

        bool RsaVerify(string value, string publicKey, string sign);

        bool RsaVerify(string value, string publicKey, string sign, Encoding encoding);

        bool Rsa2Verify(string value, string publicKey, string sign);

        bool Rsa2Verify(string value, string publicKey, string sign, Encoding encoding);

        #endregion

        string GetSHA1(string value, Encoding encoding);

        string CreateRandomCode(int codeLength, bool isPurelyNumerical = true);

        bool GetLongByGuid(out long num);

        bool GetLongByGuid(Guid guid, out long num);

        long GetNowTimestamp(DateTime? date = null);
    }
}
