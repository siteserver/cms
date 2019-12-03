using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public class DbCacheRepository : IRepository
    {
        private readonly Repository<DbCache> _repository;

        public DbCacheRepository()
        {
            _repository = new Repository<DbCache>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
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

        public async Task<bool> IsExistsAsync(string cacheKey)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(DbCache.CacheKey), cacheKey));
        }

        public async Task<string> GetValueAsync(string cacheKey)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(DbCache.CacheValue))
                .Where(nameof(DbCache.CacheKey), cacheKey));
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

        public async Task<int> GetCountAsync()
        {
            return await _repository.CountAsync();
        }

        public async Task DeleteExcess90DaysAsync()
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(DbCache.CreatedDate), "<", DateTime.Now.AddDays(-90)));
        }
    }
}
