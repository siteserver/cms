using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class User
    {
        public static string GetDisplayName(string userName, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(User), nameof(GetDisplayName), guid, userName);
            var retval = Utils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.UserDao.GetDisplayName(userName);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
