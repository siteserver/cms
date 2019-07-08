using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IUserRoleRepository : IRepository
    {
        IList<int> GetUserNameListByRoleName(int roleId);

        Task RemoveUserAsync(int userId);

        Task RemoveUserFromRoleAsync(int userId, int roleId);

        bool IsUserInRole(int userId, int roleId);

        int AddUserToRole(int userId, int roleId);

        Task<IEnumerable<int>> GetRolesAsync(int userId);
    }
}
