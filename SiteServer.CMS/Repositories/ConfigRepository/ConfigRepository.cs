using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Microsoft.Extensions.Caching.Distributed;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;

namespace SiteServer.CMS.Repositories
{
    public partial class ConfigRepository : IRepository
    {
        private readonly Repository<Config> _repository;
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;

        public ConfigRepository()
        {
            _repository = new Repository<Config>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
            _cache = CacheManager.Cache;
            _cacheKey = CacheManager.GetEntityKey(TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Config config)
        {
            var configId = await _repository.InsertAsync(config);
            await RemoveCacheAsync();

            return configId;
        }

		public async Task UpdateAsync(Config config)
		{
            await _repository.UpdateAsync(config);
            await RemoveCacheAsync();
        }

		public async Task<bool> IsInitializedAsync()
		{
            try
            {
                return await _repository.ExistsAsync();
            }
		    catch
		    {
		        // ignored
		    }

            return false;
        }
    }
}
