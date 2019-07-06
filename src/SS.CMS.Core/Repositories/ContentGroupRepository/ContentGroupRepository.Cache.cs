using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentGroupRepository
    {
        public async Task<Dictionary<int, List<ContentGroupInfo>>> GetAllContentGroupsAsync()
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async options =>
            {
                return await GetAllContentGroupsToCacheAsync();
            });
        }

        public async Task<bool> IsExistsAsync(int siteId, string groupName)
        {
            var list = await GetContentGroupInfoListAsync(siteId);
            return list.Any(group => group.GroupName == groupName);
        }

        public async Task<ContentGroupInfo> GetContentGroupInfoAsync(int siteId, string groupName)
        {
            var list = await GetContentGroupInfoListAsync(siteId);
            return list.FirstOrDefault(group => group.GroupName == groupName);
        }

        public async Task<List<string>> GetGroupNameListAsync(int siteId)
        {
            var list = await GetContentGroupInfoListAsync(siteId);
            return list.Select(group => group.GroupName).ToList();
        }

        public async Task<List<ContentGroupInfo>> GetContentGroupInfoListAsync(int siteId)
        {
            List<ContentGroupInfo> list = null;
            var dict = await GetAllContentGroupsAsync();

            if (dict != null && dict.ContainsKey(siteId))
            {
                list = dict[siteId];
            }
            return list ?? new List<ContentGroupInfo>();
        }
    }
}
