using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IWxMenuRepository : IRepository
    {
        Task<int> InsertAsync(WxMenu wxMenu);

        Task UpdateAsync(WxMenu wxMenu);

        Task DeleteAsync(int siteId, int menuId);

        Task DeleteAllAsync(int siteId);

        Task<List<WxMenu>> GetMenusAsync(int siteId);

        Task<WxMenu> GetAsync(int siteId, int id);
    }
}