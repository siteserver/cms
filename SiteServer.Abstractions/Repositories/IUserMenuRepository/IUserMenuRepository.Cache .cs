using System.Collections.Generic;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IUserMenuRepository
    {
        Task<List<UserMenu>> GetAllUserMenusAsync();

        Task<UserMenu> GetUserMenuInfoAsync(int menuId);
    }
}
