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
            var cacheKey = Utils.GetCacheKey(nameof(Tracking), nameof(GetTotalAccessNumByPageInfo), guid, publishmentSystemId.ToString(), channelId.ToString(),
                contentId.ToString(), sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalAccessNumByPageInfo(publishmentSystemId, channelId,
                contentId, sinceDate);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalAccessNumByPageUrl(int publishmentSystemId, string referrer, DateTime sinceDate, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Tracking), nameof(GetTotalAccessNumByPageUrl), guid, publishmentSystemId.ToString(), referrer, sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalAccessNumByPageUrl(publishmentSystemId, referrer, sinceDate);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalAccessNum(int publishmentSystemId, DateTime sinceDate, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Tracking), nameof(GetTotalAccessNum), guid, publishmentSystemId.ToString(), sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalAccessNum(publishmentSystemId, sinceDate);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalUniqueAccessNumByPageInfo(int publishmentSystemId, int channelId, int contentId, DateTime sinceDate, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Tracking), nameof(GetTotalUniqueAccessNumByPageInfo), guid, publishmentSystemId.ToString(), channelId.ToString(),
                contentId.ToString(), sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalUniqueAccessNumByPageInfo(publishmentSystemId,
                channelId, contentId, sinceDate);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalUniqueAccessNumByPageUrl(int publishmentSystemId, string referrer, DateTime sinceDate, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Tracking), nameof(GetTotalUniqueAccessNumByPageUrl), guid, publishmentSystemId.ToString(), referrer, sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalUniqueAccessNumByPageUrl(publishmentSystemId, referrer, sinceDate);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetTotalUniqueAccessNum(int publishmentSystemId, DateTime sinceDate, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Tracking), nameof(GetTotalUniqueAccessNum), guid, publishmentSystemId.ToString(), sinceDate.ToString(CultureInfo.InvariantCulture));
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetTotalUniqueAccessNum(publishmentSystemId, sinceDate);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetCurrentVisitorNum(int publishmentSystemId, int trackerCurrentMinute, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Tracking), nameof(GetCurrentVisitorNum), guid, publishmentSystemId.ToString(), trackerCurrentMinute.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.TrackingDao.GetCurrentVisitorNum(publishmentSystemId, trackerCurrentMinute);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static List<int> GetPageNodeIdListByAccessNum(int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Tracking), nameof(GetPageNodeIdListByAccessNum), guid, publishmentSystemId.ToString());
            var retval = Utils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.TrackingDao.GetPageNodeIdListByAccessNum(publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
        
    }
}
