using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IUserMenuRepository : IRepository
    {
        int Insert(UserMenuInfo menuInfo);

        bool Update(UserMenuInfo menuInfo);

        bool Delete(int menuId);

        List<UserMenuInfo> GetAllUserMenus();
    }
}
