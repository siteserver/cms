using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface IUserMenuRepository : IRepository
    {
        Task<int> InsertAsync(UserMenuInfo menuInfo);

        Task<bool> UpdateAsync(UserMenuInfo menuInfo);

        Task<bool> DeleteAsync(int menuId);
    }
}
