using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class AdArea
    {
        public static AdAreaInfo GetAdAreaInfo(string area, int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(AdArea), nameof(GetAdAreaInfo), area, publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<AdAreaInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.AdAreaDao.GetAdAreaInfo(area, publishmentSystemId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
