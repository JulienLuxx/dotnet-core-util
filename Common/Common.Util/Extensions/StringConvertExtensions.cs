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
        ///  StringConvertToDateTimeNullable , SafeConvertStringToNullableDateTime , return null when parse failed
        ///  <para>字符串安全转换为可空日期时间，解析失败返回 null</para>
        /// </summary>
        /// <param name="value">StringValue / 字符串值</param>
        /// <param name="format">TimeFormat , DefaultFormat : yyyyMMdd HH:mm:ss , yyyy-MM-dd HH:mm:ss , yyyy-MM-dd / 时间格式，默认格式</param>
        /// <returns>Parsed DateTime or null / 转换成功返回日期时间，失败返回 null</returns>
        public static DateTime? ConvertToDateTimeNullable(this string value, string[] format =null)
        {
            if (null == format) 
            {
                format = new string[] { "yyyyMMdd HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd" };
            }
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createTime))
            {
                return createTime;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  StringConvertToInt32 , SafeConvertStringToInt32 , return 0 when parse failed
        ///  <para>字符串安全转换为 int，解析失败返回 0</para>
        /// </summary>
        /// <param name="value">StringValue / 字符串值</param>
        /// <returns>Parsed int or 0 / 转换成功返回整数，失败返回 0</returns>
        public static int ConvertToInt32(this string value) => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) ? n : 0;

        /// <summary>
        ///  StringConvertToDecimal , SafeConvertStringToDecimal , return 0 when parse failed
        ///  <para>字符串安全转换为 decimal，解析失败返回 0</para>
        /// </summary>
        /// <param name="value">StringValue / 字符串值</param>
        /// <returns>Parsed decimal or 0 / 转换成功返回小数，失败返回 0</returns>
        public static decimal ConvertToDecimal(this string value) => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal n) ? n : 0m;

        /// <summary>
        /// StringFilterOutEmptyAndWhiteSpace , FilterOutEmptyAndWhiteSpace , return string.Empty when value is null or whitespace
        /// <para>过滤空字符串与空白字符串</para>
        /// </summary>
        /// <param name="value">StringValue / 字符串值</param>
        /// <param name="isTrim">WhetherNeedTrim / 是否去除两端空格</param>
        /// <returns>filter empty or whitespace result value / 为空或空白时返回 string.Empty，否则返回原值或 Trim 后的值</returns>
        public static string FilterEmptyWhiteSpace(this string value, bool isTrim = false) => string.IsNullOrWhiteSpace(value) ? string.Empty : isTrim ? value.Trim() : value;

        /// <summary>
        /// StringFilterOutEmptyAndWhiteSpace , FilterOutEmptyAndWhiteSpace , return false when value is null or whitespace
        /// <para>过滤空字符串与空白字符串</para>
        /// </summary>
        /// <param name="value">StringValue / 字符串值</param>
        /// <param name="filterValue">Out StringValue / 输出的过滤结果</param>
        /// <param name="isTrim">WhetherNeedTrim / 是否去除两端空格</param>
        /// <returns>If value be empty or whitespace, return false / 为空或空白时返回 false，否则返回 true</returns>
        public static bool FilterEmptyWhiteSpace(this string value, out string filterValue, bool isTrim = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                filterValue = string.Empty;
                return false;
            }

            filterValue = isTrim ? value.Trim() : value;
            return true;
        }

        /// <summary>
        ///  StringFilterOutEmptyAndWhiteSpaceOrSpecifyPhrase , FilterOutEmptyAndWhiteSpaceOrSpecifyPhrase , return string.Empty when value is null or whitespace or equals phrase
        ///  <para>过滤空字符串、空白字符串或指定短语</para>
        /// </summary>
        /// <param name="value">StringValue / 字符串值</param>
        /// <param name="phrase">SpecifyPhrase / 指定短语</param>
        /// <param name="isTrim">WhetherNeedTrim / 是否去除两端空格</param>
        /// <returns>Filter result / 命中时返回 string.Empty，否则返回原值或 Trim 后的值</returns>
        public static string FilterEmptyWhiteSpaceOrPhrase(this string value, string phrase, bool isTrim = false)  => string.IsNullOrWhiteSpace(value) || value.Trim().Equals(phrase) ? string.Empty : isTrim ? value.Trim() : value;

        #endregion

        /// <summary>
        ///  SafeConvertToString , Trim both ends , return "" when value is null
        ///  <para>安全转换为字符串，去除两端空格，值为 null 时返回 ""</para>
        /// </summary>
        /// <param name="input">InputValue / 输入值</param>
        public static string SafeString(this object input)
        {
            return input?.ToString()?.Trim() ?? string.Empty;
        }
    }
}
