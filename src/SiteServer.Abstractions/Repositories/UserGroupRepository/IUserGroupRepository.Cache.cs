using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface IUserGroupRepository
    {
        Task<bool> IsExistsAsync(string groupName);

        Task<UserGroup> GetUserGroupAsync(int groupId);

        Task<string> GetUserGroupNameAsync(int groupId);
    }
}
