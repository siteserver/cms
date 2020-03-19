using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SSCMS
{
    public interface IRoleRepository : IRepository
    {
        Task<Role> GetRoleAsync(int roleId);

        Task<List<Role>> GetRoleListAsync();

        Task<List<Role>> GetRoleListByCreatorUserNameAsync(string creatorUserName);

        Task<List<string>> GetRoleNameListAsync();

        Task<List<string>> GetRoleNameListByCreatorUserNameAsync(string creatorUserName);

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