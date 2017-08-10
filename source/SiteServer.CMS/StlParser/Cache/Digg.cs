using System.Collections.Generic;
using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Digg
    {
        public static int[] GetCount(int publishmentSystemId, int relatedIdentity, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Digg), nameof(GetCount), publishmentSystemId.ToString(), relatedIdentity.ToString());
            var retval = StlCacheUtils.GetCache<int[]>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.DiggDao.GetCount(publishmentSystemId, relatedIdentity);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static List<int> GetRelatedIdentityListByTotal(int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Digg), nameof(GetRelatedIdentityListByTotal), publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.DiggDao.GetRelatedIdentityListByTotal(publishmentSystemId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
        
    }
}
