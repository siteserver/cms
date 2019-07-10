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
        private readonly Repository<Config> _repository;

        public ConfigRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(ConfigRepository));
            _repository = new Repository<Config>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(Config.Id);
            public const string DatabaseVersion = nameof(Config.DatabaseVersion);
            public const string UpdateDate = nameof(Config.UpdateDate);
            public const string ExtendValues = nameof(Config.ExtendValues);
        }

        public async Task InsertAsync(Config configInfo)
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

        public async Task<bool> UpdateAsync(Config configInfo)
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

        public async Task<Config> GetConfigInfoAsync()
        {
            return await _cache.GetOrCreateAsync<Config>(_cacheKey, async options =>
            {
                return await GetConfigInfoWithoutExceptionAsync();
            });
        }

        private async Task<Config> GetConfigInfoWithoutExceptionAsync()
        {
            Config info = null;

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