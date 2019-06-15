using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IUserMenuRepository : IRepository
    {
        int Insert(UserMenuInfo menuInfo);

        bool Update(UserMenuInfo menuInfo);

        bool Delete(int menuId);

        List<UserMenuInfo> GetAllUserMenus();
    }
}
