using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IUserGroupRepository : IRepository
    {
        Task<int> InsertAsync(UserGroup group);

        Task UpdateAsync(UserGroup group);

        Task DeleteAsync(int groupId);

        Task<List<UserGroup>> GetUserGroupListAsync();
    }
}