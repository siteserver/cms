using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class User
    {
        private static readonly object LockObject = new object();

        public static string GetDisplayName(string userName)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(User), nameof(GetDisplayName), userName);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.UserDao.GetDisplayName(userName);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
