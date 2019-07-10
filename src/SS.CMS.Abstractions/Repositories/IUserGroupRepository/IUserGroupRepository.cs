using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IUserGroupRepository : IRepository
    {
        Task<int> InsertAsync(UserGroup groupInfo);

        Task<bool> UpdateAsync(UserGroup groupInfo);

        Task<bool> DeleteAsync(int groupId);
    }
}
