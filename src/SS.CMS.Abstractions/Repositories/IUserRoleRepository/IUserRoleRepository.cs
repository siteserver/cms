using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IUserRoleRepository : IRepository
    {
        IList<string> GetUserNameListByRoleName(string roleName);

        Task<IList<string>> GetRolesAsync(string userName);

        Task RemoveUserAsync(string userName);

        Task RemoveUserFromRoleAsync(string userName, string roleName);

        bool IsUserInRole(string userName, string roleName);

        int AddUserToRole(string userName, string roleName);
    }
}
