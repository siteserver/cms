using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IRoleRepository : IRepository
    {
        Task<Role> GetRoleAsync(int roleId);

        Task<List<Role>> GetRolesAsync();

        Task<List<Role>> GetRolesByCreatorUserNameAsync(string creatorUserName);

        Task<List<string>> GetRoleNamesAsync();

        Task<List<string>> GetRoleNamesByCreatorUserNameAsync(string creatorUserName);

        Task<int> InsertRoleAsync(Role role);

        Task UpdateRoleAsync(Role role);

        Task<bool> DeleteRoleAsync(int roleId);

        Task<bool> IsRoleExistsAsync(string roleName);

        bool IsPredefinedRole(string roleName);

        bool IsConsoleAdministrator(IList<string> roles);

        bool IsConsoleAdministrator(string role);

        bool IsSystemAdministrator(IList<string> roles);

        bool IsSystemAdministrator(string role);
    }
}