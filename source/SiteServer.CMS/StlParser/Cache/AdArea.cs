using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class AdArea
    {
        public static AdAreaInfo GetAdAreaInfo(string area, int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(AdArea), nameof(GetAdAreaInfo), guid, area, publishmentSystemId.ToString());
            var retval = Utils.GetCache<AdAreaInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.AdAreaDao.GetAdAreaInfo(area, publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
