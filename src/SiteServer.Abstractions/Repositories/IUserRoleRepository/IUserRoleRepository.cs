using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
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
