using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface IUserMenuRepository : IRepository
    {
        Task<int> InsertAsync(UserMenu menuInfo);

        Task<bool> UpdateAsync(UserMenu menuInfo);

        Task<bool> DeleteAsync(int menuId);
    }
}
