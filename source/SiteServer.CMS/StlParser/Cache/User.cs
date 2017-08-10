using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class User
    {
        public static string GetDisplayName(string userName, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(User), nameof(GetDisplayName), userName);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.UserDao.GetDisplayName(userName);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
