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
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.SiteDao.GetIdByIsRoot();
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static int GetSiteIdBySiteDir(string siteDir)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlSiteCache),
                       nameof(GetSiteIdBySiteDir), siteDir);
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal =
                        DataProvider.SiteDao.GetIdBySiteDir(
                            siteDir);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        
    }
}
