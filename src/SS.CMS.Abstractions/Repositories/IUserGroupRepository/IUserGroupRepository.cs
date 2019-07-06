using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IUserGroupRepository : IRepository
    {
        Task<int> InsertAsync(UserGroupInfo groupInfo);

        Task<bool> UpdateAsync(UserGroupInfo groupInfo);

        Task<bool> DeleteAsync(int groupId);
    }
}
