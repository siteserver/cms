using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentGroupRepository 
    {
        private async Task RemoveCacheAsync(int siteId)
        {
            var cacheKey = _cache.GetListKey(this, siteId);
            await _cache.RemoveAsync(cacheKey);
        }

        public async Task<IEnumerable<string>> GetGroupNamesAsync(int siteId)
        {
            var cacheKey = _cache.GetListKey(this, siteId);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAllAsync<string>(Q
                .Select(nameof(ContentGroup.GroupName))
                .Where(nameof(ContentGroup.SiteId), siteId)
                .OrderByDesc(nameof(ContentGroup.Taxis))
                .OrderBy(nameof(ContentGroup.GroupName))
            ));
        }
    }
}