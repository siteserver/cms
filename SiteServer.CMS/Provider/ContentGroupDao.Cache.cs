using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Core;

namespace SiteServer.CMS.Provider
{
    public partial class ContentGroupDao 
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