using System.Threading.Tasks;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Repositories;

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
                retVal = await DataProvider.SiteRepository.GetIdByIsRootAsync();
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
                    await DataProvider.SiteRepository.GetIdBySiteDirAsync(siteDir);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }

        
    }
}
