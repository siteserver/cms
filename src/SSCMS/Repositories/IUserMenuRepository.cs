using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IUserMenuRepository : IRepository
    {
        Task<int> InsertAsync(UserMenu userMenu);

        Task UpdateAsync(UserMenu userMenu);

        Task DeleteAsync(int menuId);

        Task<List<UserMenu>> GetUserMenuListAsync();

        Task<UserMenu> GetAsync(int id);
    }
}