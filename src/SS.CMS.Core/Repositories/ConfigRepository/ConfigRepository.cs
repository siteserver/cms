using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly Repository<ConfigInfo> _repository;

        public ConfigRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(ConfigRepository));
            _repository = new Repository<ConfigInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(ConfigInfo.Id);
            public const string DatabaseVersion = nameof(ConfigInfo.DatabaseVersion);
            public const string UpdateDate = nameof(ConfigInfo.UpdateDate);
            public const string ExtendValues = nameof(ConfigInfo.ExtendValues);
        }

        public async Task InsertAsync(ConfigInfo configInfo)
        {
            if (!await _repository.ExistsAsync())
            {
                configInfo.Id = await _repository.InsertAsync(configInfo);
                if (configInfo.Id > 0)
                {
                    await _cache.RemoveAsync(_cacheKey);
                }
            }
        }

        public async Task<bool> UpdateAsync(ConfigInfo configInfo)
        {
            var updated = await _repository.UpdateAsync(configInfo);
            if (updated)
            {
                await _cache.RemoveAsync(_cacheKey);
            }

            return updated;
        }

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAsync();
            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task<ConfigInfo> GetConfigInfoAsync()
        {
            return await _cache.GetOrCreateAsync<ConfigInfo>(_cacheKey, async options =>
            {
                return await GetConfigInfoWithoutExceptionAsync();
            });
        }

        private async Task<ConfigInfo> GetConfigInfoWithoutExceptionAsync()
        {
            ConfigInfo info = null;

            try
            {
                info = await _repository.GetAsync(Q.OrderBy(Attr.Id));
            }
            catch
            {
                try
                {
                    info = await _repository.GetAsync(Q.OrderBy(Attr.Id));
                }
                catch
                {
                    // ignored
                }
            }

            return info;
        }
    }
}