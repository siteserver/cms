using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Context;

namespace SiteServer.CMS.Repositories
{
    public partial class SiteRepository
    {
        public async Task<Site> GetAsync(int siteId)
        {
            if (siteId <= 0) return null;

            return await GetCacheEntityAsync(siteId);
        }

        public async Task<int> GetParentSiteIdAsync(int siteId)
        {
            var parentSiteId = 0;
            var site = await GetAsync(siteId);
            if (site != null && site.Root == false)
            {
                parentSiteId = site.ParentId;
                if (parentSiteId == 0)
                {
                    parentSiteId = await GetIdByIsRootAsync();
                }
            }
            return parentSiteId;
        }

        public async Task<int> GetSiteLevelAsync(int siteId)
        {
            var level = 0;
            var site = await GetAsync(siteId);
            if (site.ParentId != 0)
            {
                level++;
                level += await GetSiteLevelAsync(site.ParentId);
            }
            return level;
        }

        public async Task<string> GetSiteNameAsync(Site site)
        {
            var padding = string.Empty;

            var level = await GetSiteLevelAsync(site.Id);
            string psLogo;
            if (site.Root)
            {
                psLogo = "siteHQ.gif";
            }
            else
            {
                psLogo = "site.gif";
                if (level > 0 && level < 10)
                {
                    psLogo = $"subsite{level + 1}.gif";
                }
            }
            psLogo = SiteServerAssets.GetIconUrl("tree/" + psLogo);

            for (var i = 0; i < level; i++)
            {
                padding += "¡¡";
            }
            if (level > 0)
            {
                padding += "©¸ ";
            }

            return $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;{site.SiteName}";
        }

        public async Task<string> GetTableNameAsync(int siteId)
        {
            var site = await GetAsync(siteId);
            return site?.TableName;
        }

        private async Task RemoveCacheEntityAsync(int siteId)
        {
            var cacheKey = CacheManager.Cache.GetEntityKey(this, siteId);
            await CacheManager.Cache.RemoveAsync(cacheKey);
        }

        private async Task<Site> GetCacheEntityAsync(int siteId)
        {
            if (siteId == 0) return null;

            var cacheKey = CacheManager.Cache.GetEntityKey(this, siteId);
            return await CacheManager.Cache.GetOrCreateAsync(cacheKey, async options =>
                await _repository.GetAsync<Site>(Q
                    .Where(nameof(Site.Id), siteId)
                )
            );
        }

        public async Task<Site> GetSiteBySiteNameAsync(string siteName)
        {
            var list = await GetCacheListAsync();

            foreach (var cache in list)
            {
                if (StringUtils.EqualsIgnoreCase(cache.SiteName, siteName))
                {
                    return await GetAsync(cache.Id);
                }
            }
            return null;
        }

        public async Task<Site> GetSiteByIsRootAsync()
        {
            var list = await GetCacheListAsync();

            foreach (var cache in list)
            {
                if (cache.Root)
                {
                    return await GetAsync(cache.Id);
                }
            }
            return null;
        }

        public async Task<bool> IsRootExistsAsync()
        {
            var list = await GetCacheListAsync();

            foreach (var cache in list)
            {
                if (cache.Root)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<Site> GetSiteByDirectoryAsync(string siteDir)
        {
            var list = await GetCacheListAsync();

            foreach (var cache in list)
            {
                if (StringUtils.EqualsIgnoreCase(cache.SiteDir, siteDir))
                {
                    return await GetAsync(cache.Id);
                }
            }
            return null;
        }
    }
}