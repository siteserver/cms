using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IUserRoleRepository : IRepository
    {
        Task<IEnumerable<int>> GetUserNameListByRoleNameAsync(int roleId);

        Task RemoveUserAsync(int userId);

        Task RemoveUserFromRoleAsync(int userId, int roleId);

        Task<bool> IsUserInRoleAsync(int userId, int roleId);

        Task<int> AddUserToRoleAsync(int userId, int roleId);

        Task<IEnumerable<int>> GetRolesAsync(int userId);
    }
}
