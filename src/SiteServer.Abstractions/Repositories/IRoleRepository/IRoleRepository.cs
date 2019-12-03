using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface IRoleRepository : IRepository
    {
        Task<string> GetRoleDescriptionAsync(string roleName);

        Task<IEnumerable<string>> GetRoleNameListAsync();

        Task<IEnumerable<string>> GetRoleNameListByUserIdAsync(int userId);

        Task<int> InsertAsync(Role roleInfo);

        Task UpdateAsync(string roleName, string description);

        Task DeleteRoleAsync(string roleName);

        Task<bool> IsRoleExistsAsync(string roleName);
    }
}