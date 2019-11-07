using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlSiteCache
    {
        public static async Task<int> GetSiteIdByIsRootAsync()
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlSiteCache),
                       nameof(GetSiteIdByIsRootAsync));
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal == -1)
            {
                retVal = await DataProvider.SiteDao.GetIdByIsRootAsync();
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }

        public static async Task<int> GetSiteIdBySiteDirAsync(string siteDir)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlSiteCache),
                       nameof(GetSiteIdBySiteDirAsync), siteDir);
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal == -1)
            {
                retVal =
                    await DataProvider.SiteDao.GetIdBySiteDirAsync(siteDir);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }

        
    }
}
