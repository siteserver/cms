using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IUserGroupRepository
    {
        Task<bool> IsExistsAsync(string groupName);

        Task<UserGroup> GetUserGroupAsync(int groupId);

        Task<string> GetUserGroupNameAsync(int groupId);
    }
}
