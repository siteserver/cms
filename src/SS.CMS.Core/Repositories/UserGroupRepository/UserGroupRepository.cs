using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class UserGroupRepository : IUserGroupRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly Repository<UserGroup> _repository;

        public UserGroupRepository(IDistributedCache cache, ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(UserGroupRepository));
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _repository = new Repository<UserGroup>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(UserGroup.Id);
        }

        public async Task<int> InsertAsync(UserGroup groupInfo)
        {
            groupInfo.Id = await _repository.InsertAsync(groupInfo);

            await _cache.RemoveAsync(_cacheKey);

            return groupInfo.Id;
        }

        public async Task<bool> UpdateAsync(UserGroup groupInfo)
        {
            var updated = await _repository.UpdateAsync(groupInfo);

            await _cache.RemoveAsync(_cacheKey);

            return updated;
        }

        public async Task<bool> DeleteAsync(int groupId)
        {
            var deleted = await _repository.DeleteAsync(groupId);

            await _cache.RemoveAsync(_cacheKey);

            return deleted;
        }

        private async Task<IEnumerable<UserGroup>> GetUserGroupInfoListToCacheAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(Attr.Id));
        }
    }
}
