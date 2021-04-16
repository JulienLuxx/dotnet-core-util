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
