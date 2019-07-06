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
        private readonly Repository<UserGroupInfo> _repository;

        public UserGroupRepository(IDistributedCache cache, ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(UserGroupRepository));
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _repository = new Repository<UserGroupInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(UserGroupInfo.Id);
        }

        public async Task<int> InsertAsync(UserGroupInfo groupInfo)
        {
            groupInfo.Id = _repository.Insert(groupInfo);

            await _cache.RemoveAsync(_cacheKey);

            return groupInfo.Id;
        }

        public async Task<bool> UpdateAsync(UserGroupInfo groupInfo)
        {
            var updated = _repository.Update(groupInfo);

            await _cache.RemoveAsync(_cacheKey);

            return updated;
        }

        public async Task<bool> DeleteAsync(int groupId)
        {
            var deleted = await _repository.DeleteAsync(groupId);

            await _cache.RemoveAsync(_cacheKey);

            return deleted;
        }

        private async Task<IEnumerable<UserGroupInfo>> GetUserGroupInfoListToCacheAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(Attr.Id));
        }
    }
}
