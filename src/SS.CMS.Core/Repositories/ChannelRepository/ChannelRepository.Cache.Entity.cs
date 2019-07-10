using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using Attr = SS.CMS.Core.Models.Attributes.ChannelAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        private async Task RemoveEntityCacheAsync(int channelId)
        {
            var cacheKey = _cache.GetEntityKey(this, channelId);
            await _cache.RemoveAsync(cacheKey);
        }

        private async Task<Channel> GetEntityCacheAsync(int channelId)
        {
            if (channelId == 0) return null;

            var cacheKey = _cache.GetEntityKey(this, channelId);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await _repository.GetAsync<Channel>(Q
                .Where(Attr.Id, channelId));
            });
        }
    }
}