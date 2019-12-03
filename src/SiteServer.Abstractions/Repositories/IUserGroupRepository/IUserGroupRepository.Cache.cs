using System.Collections.Generic;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IUserGroupRepository
    {
        Task<IList<UserGroup>> GetAllUserGroupsAsync();

        Task<bool> IsExistsAsync(string groupName);

        Task<UserGroup> GetUserGroupInfoAsync(int groupId);
    }
}
