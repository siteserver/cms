using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class UsersInGroupsRepository : IUsersInGroupsRepository
    {
        private readonly Repository<UsersInGroups> _repository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UsersInGroupsRepository(ISettingsManager settingsManager, IUserGroupRepository userGroupRepository)
        {
            _repository = new Repository<UsersInGroups>(settingsManager.Database, settingsManager.Redis);
            _userGroupRepository = userGroupRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<IList<int>> GetGroupIdsAsync(int userId)
        {
            return await _repository.GetAllAsync<int>(Q
                .Distinct()
                .Select(nameof(UsersInGroups.GroupId))
                .Where(nameof(UsersInGroups.UserId), userId)
                .OrderBy(nameof(UsersInGroups.GroupId))
            );
        }

        public async Task<IList<int>> GetUserIdsAsync(int groupId)
        {
            return await _repository.GetAllAsync<int>(Q
                .Distinct()
                .Select(nameof(UsersInGroups.UserId))
                .Where(nameof(UsersInGroups.GroupId), groupId)
                .OrderBy(nameof(UsersInGroups.UserId))
            );
        }

        public async Task<IList<int>> GetUserIdsAsync()
        {
            return await _repository.GetAllAsync<int>(Q
                .Distinct()
                .Select(nameof(UsersInGroups.UserId))
                .OrderBy(nameof(UsersInGroups.UserId))
            );
        }

        public async Task<int> GetCountAsync(int groupId)
        {
            return await _repository.CountAsync(Q
                .Where(nameof(UsersInGroups.GroupId), groupId)
            );
        }

        public async Task DeleteAsync(int groupId, int userId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(UsersInGroups.UserId), userId)
                .Where(nameof(UsersInGroups.GroupId), groupId)
            );
        }

        public async Task<bool> IsExistsAsync(int groupId, int userId)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(UsersInGroups.UserId), userId)
                .Where(nameof(UsersInGroups.GroupId), groupId)
            );
        }

        public async Task InsertIfNotExistsAsync(int groupId, int userId)
        {
            if (!await IsExistsAsync(groupId, userId))
            {
                await _repository.InsertAsync(new UsersInGroups
                {
                    GroupId = groupId,
                    UserId = userId,
                });
            }
        }

        public async Task<List<UserGroup>> GetGroupsAsync(User user)
        {
            var groups = new List<UserGroup>();

            if (user == null)
            {
                return groups;
            }

            var groupIds = await GetGroupIdsAsync(user.Id);
            var allGroups = await _userGroupRepository.GetUserGroupsAsync(true);

            if (groupIds.Count > 0)
            {
                groups = allGroups.Where(g => groupIds.Contains(g.Id)).ToList();
            }

            if (user.Manager)
            {
                var group = allGroups.Where(g => g.Id == _userGroupRepository.GroupIdOfManager).ToList();
                groups.AddRange(group);
            }
            
            if (groups.Count == 0)
            {
                groups = allGroups.Where(g => g.Id == _userGroupRepository.GroupIdOfDefault).ToList();
            }

            return groups;
        }
    }
}
