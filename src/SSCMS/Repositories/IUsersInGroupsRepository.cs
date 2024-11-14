using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IUsersInGroupsRepository : IRepository
    {
        Task<IList<int>> GetGroupIdsAsync(int userId);

        Task<IList<int>> GetUserIdsAsync(int groupId);

        Task<IList<int>> GetUserIdsAsync();

        Task<int> GetCountAsync(int groupId);

        Task DeleteAsync(int groupId, int userId);

        Task DeleteAllByGroupIdAsync(int groupId);

        Task DeleteAllByUserIdAsync(int userId);

        Task<bool> IsExistsAsync(int groupId, int userId);

        Task InsertIfNotExistsAsync(int groupId, int userId);

        Task<List<UserGroup>> GetGroupsAsync(User user);
    }
}
