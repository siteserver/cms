using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Core.Repositories
{
    public partial class UserGroupRepository
    {
        public async Task<IList<UserGroupInfo>> GetAllUserGroupsAsync()
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async options =>
            {
                var list = new List<UserGroupInfo>();
                list.Add(new UserGroupInfo
                {
                    Id = 0,
                    GroupName = "默认用户组",
                    AdminName = string.Empty
                });
                var userGroupInfoList = await GetUserGroupInfoListToCacheAsync();
                if (userGroupInfoList != null)
                {
                    list.AddRange(userGroupInfoList);
                }
                return list;
            });
        }

        public async Task<bool> IsExistsAsync(string groupName)
        {
            var list = await GetAllUserGroupsAsync();
            return list.Any(group => group.GroupName == groupName);
        }

        public async Task<UserGroupInfo> GetUserGroupInfoAsync(int groupId)
        {
            var list = await GetAllUserGroupsAsync();
            return list.FirstOrDefault(group => group.Id == groupId) ?? list[0];
        }
    }
}
