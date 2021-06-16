using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Common.Util
{
    public static class StringConvertExtensions
    {
        #region

        /// <summary>
        ///  StringConvertToDateTimeNullable
        /// </summary>
        /// <param name="value">StringValue</param>
        /// <param name="format">TimeFormat , DefaultFormat : yyyyMMdd HH:mm:ss , yyyy-MM-dd</param>
        /// <returns></returns>
        public static DateTime? ConvertToDateTimeNullable(this string value, string[] format =null)
        {
            if (null == format) 
            {
                format = new string[] { "yyyyMMdd HH:mm:ss", "yyyy-MM-dd" };
            }
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime createTime))
            {
                return createTime;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  StringConvertToInt32
        /// </summary>
        /// <param name="value">StringValue</param>
        /// <returns></returns>
        public static int ConvertToInt32(this string value) => int.TryParse(value, out int n) ? n : 0;

        /// <summary>
        ///  StringConvertToDecimal
        /// </summary>
        /// <param name="value">StringValue</param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(this string value) => decimal.TryParse(value, out decimal n) ? n : 0m;

        /// <summary>
        /// StringFilterOutEmptyAndWhiteSpace
        /// </summary>
        /// <param name="value">StringValue</param>
        /// <param name="isTrim">WhetherNeedTrim</param>
        /// <returns></returns>
        public static string FilterEmptyWhiteSpace(this string value, bool isTrim = false) => string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? string.Empty : isTrim ? value.Trim() : value;

        /// <summary>
        /// StringFilterOutEmptyAndWhiteSpace
        /// </summary>
        /// <param name="value">StringValue</param>
        /// <param name="filterValue">Out StringValue</param>
        /// <param name="isTrim">WhetherNeedTrim</param>
        /// <returns></returns>
        public static bool FilterEmptyWhiteSpace(this string value, out string filterValue, bool isTrim = false)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                filterValue = string.Empty;
                return false;
            }
            else
            {
                if (isTrim)
                {
                    filterValue = value.Trim();
                    return true;
                }
                else
                {
                    filterValue = value;
                    return true;
                }
            }
        }

        /// <summary>
        ///  StringFilterOutEmptyAndWhiteSpaceOrSpecifyPhrase
        /// </summary>
        /// <param name="value">StringValue</param>
        /// <param name="phrase">SpecifyPhrase</param>
        /// <param name="isTrim">WhetherNeedTrim</param>
        /// <returns></returns>
        public static string FilterEmptyWhiteSpaceOrPhrase(this string value, string phrase, bool isTrim = false)  => string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) || value.Trim().Equals(phrase) ? string.Empty : isTrim ? value.Trim() : value;

        #endregion
    }
}
