using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public class DbCacheRepository : IDbCacheRepository
    {
        private readonly Repository<DbCache> _repository;

        public DbCacheRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<DbCache>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task RemoveAndInsertAsync(string cacheKey, string cacheValue)
        {
            if (string.IsNullOrEmpty(cacheKey)) return;

            await DeleteExcess90DaysAsync();

            await _repository.DeleteAsync(Q
                .Where(nameof(DbCache.CacheKey), cacheKey));

            await _repository.InsertAsync(new DbCache
            {
                CacheKey = cacheKey,
                CacheValue = cacheValue
            });
        }

        public async Task ClearAsync()
        {
            await _repository.DeleteAsync();
        }

        public async Task<string> GetValueAndRemoveAsync(string cacheKey)
        {
            var retVal = await _repository.GetAsync<string>(Q
                .Select(nameof(DbCache.CacheValue))
                .Where(nameof(DbCache.CacheKey), cacheKey));

            await _repository.DeleteAsync(Q
                .Where(nameof(DbCache.CacheKey), cacheKey));

            return retVal;
        }

        private async Task DeleteExcess90DaysAsync()
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(DbCache.CreatedDate), "<", DateTime.Now.AddDays(-90)));
        }
    }
}
