using System.Collections.Generic;
using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Digg
    {
        public static int[] GetCount(int publishmentSystemId, int relatedIdentity, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Digg), nameof(GetCount), guid, publishmentSystemId.ToString(), relatedIdentity.ToString());
            var retval = Utils.GetCache<int[]>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.DiggDao.GetCount(publishmentSystemId, relatedIdentity);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static List<int> GetRelatedIdentityListByTotal(int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Digg), nameof(GetRelatedIdentityListByTotal), guid, publishmentSystemId.ToString());
            var retval = Utils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.DiggDao.GetRelatedIdentityListByTotal(publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
        
    }
}
