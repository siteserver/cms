using System.Collections.Generic;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Star
    {
        public static int[] GetCount(int publishmentSystemId, int channelId, int contentId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Star), nameof(GetCount), guid, publishmentSystemId.ToString(), channelId.ToString(), contentId.ToString());
            var retval = Utils.GetCache<int[]>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.StarDao.GetCount(publishmentSystemId, channelId, contentId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static List<int> GetContentIdListByPoint(int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Star), nameof(GetContentIdListByPoint), guid, publishmentSystemId.ToString());
            var retval = Utils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.StarDao.GetContentIdListByPoint(publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
        
    }
}
