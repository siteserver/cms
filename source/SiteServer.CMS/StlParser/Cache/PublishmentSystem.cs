using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class PublishmentSystem
    {
        public static int GetPublishmentSystemIdByIsHeadquarters(string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(PublishmentSystem), nameof(GetPublishmentSystemIdByIsHeadquarters));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
