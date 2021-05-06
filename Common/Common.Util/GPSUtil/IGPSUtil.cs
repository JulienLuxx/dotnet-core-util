using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    public interface IGPSUtil
    {
        decimal CalculateDistance(double lng, double lat, double dlng, double dlat);
    }
}
