using System.Collections.Generic;
using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Digg
    {
        private static readonly object LockObject = new object();

        public static int[] GetCount(int publishmentSystemId, int relatedIdentity)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Digg), nameof(GetCount),
                    publishmentSystemId.ToString(), relatedIdentity.ToString());
            var retval = StlCacheUtils.GetCache<int[]>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<int[]>(cacheKey);
                if (retval == null)
                {
                    retval = BaiRongDataProvider.DiggDao.GetCount(publishmentSystemId, relatedIdentity);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<int> GetRelatedIdentityListByTotal(int publishmentSystemId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Digg), nameof(GetRelatedIdentityListByTotal),
                    publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = BaiRongDataProvider.DiggDao.GetRelatedIdentityListByTotal(publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
        
    }
}
