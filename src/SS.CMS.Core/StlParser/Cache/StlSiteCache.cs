using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Common;

namespace SS.CMS.Core.Cache.Stl
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
                    retval = DataProvider.SiteRepository.GetIdByIsRoot();
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
                        DataProvider.SiteRepository.GetIdBySiteDir(
                            siteDir);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        
    }
}
