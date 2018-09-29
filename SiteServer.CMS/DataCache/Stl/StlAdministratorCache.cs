using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlAdministratorCache
    {
        private static readonly object LockObject = new object();

        public static string GetDisplayName(string userName)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlAdministratorCache), nameof(GetDisplayName),
                       userName);
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.AdministratorDao.GetDisplayName(userName);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
