using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository
    {
        private async Task RemoveCacheEntityAsync(int siteId)
        {
            var cacheKey = _cache.GetEntityKey(this, siteId);
            await _cache.RemoveAsync(cacheKey);
        }

        private async Task<Site> GetCacheEntityAsync(int siteId)
        {
            if (siteId == 0) return null;

            var cacheKey = _cache.GetEntityKey(this, siteId);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await _repository.GetAsync<Site>(Q
                .Where(Attr.Id, siteId));
            });
        }

        // private string GetSiteDir(SiteInfo siteInfo)
        // {
        //     if (siteInfo == null || siteInfo.IsRoot) return string.Empty;
        //     if (siteInfo.ParentId != 0)
        //     {
        //         SiteInfo parent = null;
        //         foreach (var pair in listFromDb)
        //         {
        //             var theSiteId = pair.Key;
        //             if (theSiteId != siteInfo.ParentId) continue;
        //             parent = pair.Value;
        //             break;
        //         }
        //         return PathUtils.Combine(GetSiteDir(listFromDb, parent), PathUtils.GetDirectoryName(siteInfo.SiteDir, false));
        //     }
        //     return PathUtils.GetDirectoryName(siteInfo.SiteDir, false);
        // }
    }
}