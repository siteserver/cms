using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Site
    {
        private static readonly object LockObject = new object();

        public static int GetSiteIdByIsRoot()
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Site),
                       nameof(GetSiteIdByIsRoot));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.SiteDao.GetIdByIsRoot();
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetSiteIdBySiteDir(string siteDir)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Site),
                       nameof(GetSiteIdBySiteDir), siteDir);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval =
                        DataProvider.SiteDao.GetIdBySiteDir(
                            siteDir);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        
    }
}
