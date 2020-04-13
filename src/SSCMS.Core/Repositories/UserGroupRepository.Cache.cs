using System.Linq;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class UserGroupRepository
    {
        public async Task<bool> IsExistsAsync(string groupName)
        {
            var list = await GetUserGroupListAsync();
            return list.Any(group => group.GroupName == groupName);
        }

        public async Task<UserGroup> GetUserGroupAsync(int groupId)
        {
            var list = await GetUserGroupListAsync();
            return list.FirstOrDefault(group => group.Id == groupId) ?? list[0];
        }

        public async Task<string> GetUserGroupNameAsync(int groupId)
        {
            return (await GetUserGroupAsync(groupId)).GroupName;
        }
	}
}
