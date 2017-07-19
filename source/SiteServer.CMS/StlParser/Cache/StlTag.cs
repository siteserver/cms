using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class StlTag
    {
        public static StlTagInfo GetStlTagInfo(int publishmentSystemId, string tagName, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(StlTag), nameof(GetStlTagInfo), guid, publishmentSystemId.ToString(), tagName);
            var retval = Utils.GetCache<StlTagInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.StlTagDao.GetStlTagInfo(publishmentSystemId, tagName);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
