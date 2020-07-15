using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IOpenMenuRepository : IRepository
    {
        Task<int> InsertAsync(OpenMenu openMenu);

        Task UpdateAsync(OpenMenu openMenu);

        Task DeleteAsync(int siteId, int menuId);

        Task DeleteAllAsync(int siteId);

        Task<List<OpenMenu>> GetOpenMenusAsync(int siteId);

        Task<OpenMenu> GetAsync(int siteId, int id);
    }
}