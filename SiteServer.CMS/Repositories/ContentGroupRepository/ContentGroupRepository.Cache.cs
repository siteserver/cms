using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Datory.Caching;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentGroupRepository 
    {
        private async Task RemoveCacheAsync(int siteId)
        {
            var cacheKey = CacheManager.GetListKey(TableName, siteId);
            await _repository.Cache.RemoveAsync(cacheKey);
        }

        public async Task<IEnumerable<string>> GetGroupNamesAsync(int siteId)
        {
            var cacheKey = CacheManager.GetListKey(TableName, siteId);
            return await _repository.Cache.GetOrCreateAsync(cacheKey, async () => await _repository.GetAllAsync<string>(Q
                .Select(nameof(ContentGroup.GroupName))
                .Where(nameof(ContentGroup.SiteId), siteId)
                .OrderByDesc(nameof(ContentGroup.Taxis))
                .OrderBy(nameof(ContentGroup.GroupName))
            ));
        }
    }
}