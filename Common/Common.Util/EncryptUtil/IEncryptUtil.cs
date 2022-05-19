using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public interface IEncryptUtil : IDependency
    {
        string GetMd5By16(string value);

        string GetMd5By16(string value, Encoding encoding);

        string GetMd5By32(string value);

        string GetMd5By32(string value, Encoding encoding);

        string GetSHA1(string value, Encoding encoding);

        string AesEncrypt(string value);

        string AesEncrypt(string value, string key);

        string AesDecrypt(string value);

        string AesDecrypt(string value, string key);

        string CreateRandomCode(int codeLength, bool isPurelyNumerical = true);

        bool GetLongByGuid(out long num);

        bool GetLongByGuid(Guid guid, out long num);

        long GetNowTimestamp(DateTime? date = null);
    }
}
