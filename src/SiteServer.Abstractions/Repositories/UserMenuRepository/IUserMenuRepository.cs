using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SiteServer.Abstractions
{
    public partial interface IUserMenuRepository : IRepository
    {
        Task<int> InsertAsync(UserMenu userMenu);

        Task UpdateAsync(UserMenu userMenu);

        Task DeleteAsync(int menuId);

        Task<List<UserMenu>> GetUserMenuListAsync();
    }
}