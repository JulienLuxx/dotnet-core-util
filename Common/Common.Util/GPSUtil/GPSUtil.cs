using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public class GPSUtil : IGPSUtil
    {
        public decimal CalculateDistance(double lng, double lat, double dlng, double dlat)
        {
            var radLatBegin = lat * Math.PI / 180;
            var radLatEnd = dlat * Math.PI / 180;
            var radLatDiff = radLatBegin - radLatEnd;
            var radLngDiff = lng * Math.PI / 180 - dlng * Math.PI / 180;
            var distance = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(radLatDiff / 2), 2) + Math.Cos(radLatBegin) * Math.Cos(radLatEnd) * Math.Pow(Math.Sin(radLngDiff / 2), 2))) * R;
            return (decimal)distance * 1000;
        }
    }
}
