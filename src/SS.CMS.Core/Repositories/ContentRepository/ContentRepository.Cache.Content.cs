using System.Collections.Generic;
using SS.CMS.Models;
using SS.CMS.Utils;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private string GetContentCacheKey(int channelId)
        {
            return _cache.GetKey(nameof(ContentRepository), nameof(GetContentCacheKey), channelId.ToString());
        }

        private async Task ContentRemoveAsync(int channelId)
        {
            var cacheKey = GetContentCacheKey(channelId);
            await _cache.RemoveAsync(cacheKey);
        }

        public ContentInfo GetContentInfo(int contentId)
        {
            return GetCacheContentInfo(contentId);
        }
    }
}