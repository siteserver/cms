using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class ConfigRepository : IRepository
    {
        private readonly Repository<Config> _repository;
        private readonly string _cacheKey;

        public ConfigRepository()
        {
            _repository = new Repository<Config>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString), new Redis(WebConfigUtils.RedisConnectionString));
            _cacheKey = Caching.GetEntityKey(TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Config config)
        {
            var configId = await _repository.InsertAsync(config, Q
                .CachingRemove(_cacheKey)
            );

            return configId;
        }

		public async Task UpdateAsync(Config config)
		{
            await _repository.UpdateAsync(config, Q
                .CachingRemove(_cacheKey)
            );
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
