using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IUserGroupRepository : IRepository
    {
        int GroupIdOfDefault { get; }
        int GroupIdOfManager { get; }

        Task<int> InsertAsync(UserGroup group);

        Task UpdateAsync(UserGroup group);

        Task DeleteAsync(int groupId);

        Task<List<UserGroup>> GetUserGroupsAsync(bool isSystem);

        Task<List<UserGroup>> GetUserGroupsAsync();

        Task ClearCache();

        Task UpdateTaxisDownAsync(int groupId, int taxis);

        Task UpdateTaxisUpAsync(int groupId, int taxis);
    }
}