using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class AdArea
    {
        private static readonly object LockObject = new object();

        public static AdAreaInfo GetAdAreaInfo(string area, int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(AdArea), nameof(GetAdAreaInfo), area, publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<AdAreaInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<AdAreaInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.AdAreaDao.GetAdAreaInfo(area, publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
