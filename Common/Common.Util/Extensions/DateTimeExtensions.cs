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
    }
}
