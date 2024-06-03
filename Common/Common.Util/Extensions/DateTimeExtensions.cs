using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Common.Util
{
    public static partial class DateTimeExtensions
    {
        public static string ToISO8601FormatString(this DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.CreateSpecificCulture("en-US"));
        }

        /// <summary>
        ///  Convert to second base unix time stamp
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToShortUnixTimeStamp(this DateTime dateTime)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
            long ticks = (dateTime - start.Add(new TimeSpan(8, 0, 0))).Ticks;
            return ticks / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        ///  Convert to millisecond base unix time stamp
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToLongUnixTimeStamp(this DateTime dateTime)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
            long ticks = (dateTime - start.Add(new TimeSpan(8, 0, 0))).Ticks;
            return ticks / TimeSpan.TicksPerMillisecond;
        }

        public static DateTime ToDateTime(this long timestamp)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            TimeSpan span = new TimeSpan(long.Parse(timestamp + "0000000"));
            return start.Add(span).Add(new TimeSpan(8, 0, 0));
        }
    }
}
