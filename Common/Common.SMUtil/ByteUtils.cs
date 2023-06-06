using System;
using System.Collections.Generic;
using System.Text;

namespace Common.SMUtil
{
    public static class ByteUtils
    {
        public static string ToHexString(this byte[] datas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < datas.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", datas[i]));
            }
            return builder.ToString().Trim();
        }

        public static byte[] GetBytesByHexString(this string datas)
        {
            var result = new byte[datas.Length / 2];
            for (var x = 0; x < result.Length; x++)
            {
                result[x] = Convert.ToByte(datas.Substring(x * 2, 2), 16);
            }
            return result;
        }
    }
}
