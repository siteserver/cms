using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlSiteCache
    {
        private static readonly object LockObject = new object();

        public static int GetSiteIdByIsRoot()
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlSiteCache),
                       nameof(GetSiteIdByIsRoot));
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.SiteDao.GetIdByIsRoot();
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetSiteIdBySiteDir(string siteDir)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlSiteCache),
                       nameof(GetSiteIdBySiteDir), siteDir);
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval =
                        DataProvider.SiteDao.GetIdBySiteDir(
                            siteDir);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        
    }
}
