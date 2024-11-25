using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public partial class UserGroupRepository : IUserGroupRepository
    {
        private readonly Repository<UserGroup> _repository;

        public UserGroupRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<UserGroup>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey => CacheUtils.GetListKey(_repository.TableName);
        
        public int GroupIdOfDefault => -99;
        public int GroupIdOfManager => -98;

        public async Task<int> InsertAsync(UserGroup group)
        {
            group.Taxis = await GetMaxTaxisAsync() + 1;
            return await _repository.InsertAsync(group, Q.CachingRemove(CacheKey));
        }

        public async Task UpdateAsync(UserGroup group)
        {
            await _repository.UpdateAsync(group, Q.CachingRemove(CacheKey));
        }

        public async Task ClearCache()
        {
            await _repository.RemoveCacheAsync(CacheKey);
        }

        public async Task DeleteAsync(int groupId)
        {
            await _repository.DeleteAsync(groupId, Q.CachingRemove(CacheKey));
        }

        public async Task<List<UserGroup>> GetUserGroupsAsync()
        {
            return await GetUserGroupsAsync(false);
        }

        public async Task<List<UserGroup>> GetUserGroupsAsync(bool isSystem)
        {
            var list = (await _repository.GetAllAsync(Q
                .OrderBy(nameof(UserGroup.Taxis), nameof(UserGroup.Id))
                .CachingGet(CacheKey)
            )).ToList();

            if (isSystem)
            {
                list.Insert(0, new UserGroup
                {
                    Id = GroupIdOfManager,
                    GroupName = "主管",
                    Description = "用户被设置为部门主管后，将自动属于主管用户组",
                    IsManager = true,
                    // AdminName = config.UserManagerGroupAdminName
                });
                list.Insert(0, new UserGroup
                {
                    Id = GroupIdOfDefault,
                    GroupName = "默认",
                    Description = "所有未设置用户组的用户，将自动属于默认用户组",
                    IsDefault = true,
                    // AdminName = config.UserDefaultGroupAdminName
                });
            }

            return list;
        }

        public async Task UpdateTaxisDownAsync(int groupId, int taxis)
        {
            var higherGroup = await _repository.GetAsync<UserGroup>(Q
                .Select(nameof(UserGroup.Id), nameof(UserGroup.Taxis))
                .Where(nameof(UserGroup.Taxis), ">", taxis)
                .WhereNot(nameof(UserGroup.Id), groupId)
                .OrderBy(nameof(UserGroup.Taxis)));

            if (higherGroup != null)
            {
                await SetTaxisAsync(groupId, higherGroup.Taxis);
                await SetTaxisAsync(higherGroup.Id, taxis);
            }
        }

        public async Task UpdateTaxisUpAsync(int groupId, int taxis)
        {
            var lowerGroup = await _repository.GetAsync<UserGroup>(Q
                .Select(nameof(UserGroup.Id), nameof(UserGroup.Taxis))
                .Where(nameof(UserGroup.Taxis), "<", taxis)
                .WhereNot(nameof(UserGroup.Id), groupId)
                .OrderByDesc(nameof(UserGroup.Taxis)));

            if (lowerGroup != null)
            {
                await SetTaxisAsync(groupId, lowerGroup.Taxis);
                await SetTaxisAsync(lowerGroup.Id, taxis);
            }
        }

        private async Task SetTaxisAsync(int groupId, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(UserGroup.Taxis), taxis)
                .Where(nameof(UserGroup.Id), groupId)
                .CachingRemove(CacheKey)
            );
        }

        private async Task<int> GetMaxTaxisAsync()
        {
            var max = await _repository.MaxAsync(nameof(UserGroup.Taxis));
            return max ?? 0;
        }
    }
}
