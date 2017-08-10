using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Administrator
    {
        public static string GetDisplayName(string userName, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Administrator), nameof(GetDisplayName), userName);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.AdministratorDao.GetDisplayName(userName);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
