using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Administrator
    {
        public static string GetDisplayName(string userName, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Administrator), nameof(GetDisplayName), guid, userName);
            var retval = Utils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.AdministratorDao.GetDisplayName(userName);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
