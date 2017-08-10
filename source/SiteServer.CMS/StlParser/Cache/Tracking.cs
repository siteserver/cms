using System;
using System.Collections.Generic;
using System.Globalization;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Tracking
    {
        public static int GetTotalAccessNumByPageInfo(int publishmentSystemId, int channelId, int contentId, DateTime sinceDate, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tracking), nameof(GetTotalAccessNumByPageInfo), publishmentSystemId.ToString(), channelId.ToString(),
                contentId.ToString(), sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalAccessNumByPageInfo(publishmentSystemId, channelId,
                contentId, sinceDate);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalAccessNumByPageUrl(int publishmentSystemId, string referrer, DateTime sinceDate, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tracking), nameof(GetTotalAccessNumByPageUrl), publishmentSystemId.ToString(), referrer, sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalAccessNumByPageUrl(publishmentSystemId, referrer, sinceDate);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalAccessNum(int publishmentSystemId, DateTime sinceDate, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tracking), nameof(GetTotalAccessNum), publishmentSystemId.ToString(), sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalAccessNum(publishmentSystemId, sinceDate);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalUniqueAccessNumByPageInfo(int publishmentSystemId, int channelId, int contentId, DateTime sinceDate, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tracking), nameof(GetTotalUniqueAccessNumByPageInfo), publishmentSystemId.ToString(), channelId.ToString(),
                contentId.ToString(), sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalUniqueAccessNumByPageInfo(publishmentSystemId,
                channelId, contentId, sinceDate);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalUniqueAccessNumByPageUrl(int publishmentSystemId, string referrer, DateTime sinceDate, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tracking), nameof(GetTotalUniqueAccessNumByPageUrl), publishmentSystemId.ToString(), referrer, sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalUniqueAccessNumByPageUrl(publishmentSystemId, referrer, sinceDate);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalUniqueAccessNum(int publishmentSystemId, DateTime sinceDate, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tracking), nameof(GetTotalUniqueAccessNum), publishmentSystemId.ToString(), sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalUniqueAccessNum(publishmentSystemId, sinceDate);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetCurrentVisitorNum(int publishmentSystemId, int trackerCurrentMinute, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tracking), nameof(GetCurrentVisitorNum), publishmentSystemId.ToString(), trackerCurrentMinute.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetCurrentVisitorNum(publishmentSystemId, trackerCurrentMinute);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static List<int> GetPageNodeIdListByAccessNum(int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tracking), nameof(GetPageNodeIdListByAccessNum), publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.TrackingDao.GetPageNodeIdListByAccessNum(publishmentSystemId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
        
    }
}
