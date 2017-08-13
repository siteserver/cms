using System.Collections.Generic;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Star
    {
        private static readonly object LockObject = new object();

        public static int[] GetCount(int publishmentSystemId, int channelId, int contentId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Star), nameof(GetCount),
                    publishmentSystemId.ToString(), channelId.ToString(), contentId.ToString());
            var retval = StlCacheUtils.GetCache<int[]>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<int[]>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.StarDao.GetCount(publishmentSystemId, channelId, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<int> GetContentIdListByPoint(int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Star), nameof(GetContentIdListByPoint),
                       publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.StarDao.GetContentIdListByPoint(publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
