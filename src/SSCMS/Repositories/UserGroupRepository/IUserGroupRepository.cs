using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SSCMS
{
    public partial interface IUserGroupRepository : IRepository
    {
        Task<int> InsertAsync(UserGroup group);

        Task UpdateAsync(UserGroup group);

        Task DeleteAsync(int groupId);

        Task<List<UserGroup>> GetUserGroupListAsync();
    }
}