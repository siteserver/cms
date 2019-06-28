using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IUserMenuRepository : IRepository
    {
        int Insert(UserMenuInfo menuInfo);

        bool Update(UserMenuInfo menuInfo);

        Task<bool> DeleteAsync(int menuId);

        List<UserMenuInfo> GetAllUserMenus();
    }
}
