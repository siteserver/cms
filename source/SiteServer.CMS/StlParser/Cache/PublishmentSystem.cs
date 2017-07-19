using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class PublishmentSystem
    {
        public static int GetPublishmentSystemIdByIsHeadquarters(string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(PublishmentSystem), nameof(GetPublishmentSystemIdByIsHeadquarters), guid);
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
