using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelGroupRepository
    {
        // public void ClearCache()
        // {
        //     _cacheManager.Remove(CacheKey);
        // }

        public async Task<Dictionary<int, List<ChannelGroupInfo>>> GetAllChannelGroupsAsync()
        {
            return await _cache.GetOrCreateAsync<Dictionary<int, List<ChannelGroupInfo>>>(_cacheKey, async options =>
            {
                return await GetAllChannelGroupsToCacheAsync();
            });
        }

        public async Task<bool> IsExistsAsync(int siteId, string groupName)
        {
            var list = await GetChannelGroupInfoListAsync(siteId);
            return list.Any(group => group.GroupName == groupName);
        }

        public async Task<ChannelGroupInfo> GetChannelGroupInfoAsync(int siteId, string groupName)
        {
            var list = await GetChannelGroupInfoListAsync(siteId);
            return list.FirstOrDefault(group => group.GroupName == groupName);
        }

        public async Task<List<string>> GetGroupNameListAsync(int siteId)
        {
            var list = await GetChannelGroupInfoListAsync(siteId);
            return list.Select(group => group.GroupName).ToList();
        }

        public async Task<List<ChannelGroupInfo>> GetChannelGroupInfoListAsync(int siteId)
        {
            List<ChannelGroupInfo> list = null;
            var dict = await GetAllChannelGroupsAsync();

            if (dict != null && dict.ContainsKey(siteId))
            {
                list = dict[siteId];
            }
            return list ?? new List<ChannelGroupInfo>();
        }
    }
}
