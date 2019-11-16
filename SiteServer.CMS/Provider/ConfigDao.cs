using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class ConfigDao : IRepository
    {
        private readonly Repository<Config> _repository;

        public ConfigDao()
        {
            _repository = new Repository<Config>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Config config)
        {
            var configId = await _repository.InsertAsync(config);
            ConfigManager.IsChanged = true;
            return configId;
        }

		public async Task UpdateAsync(Config config)
		{
            await _repository.UpdateAsync(config);
            ConfigManager.IsChanged = true;
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

		public async Task<string> GetDatabaseVersionAsync()
		{
			var databaseVersion = string.Empty;

			try
			{
                databaseVersion = await _repository.GetAsync<string>(Q
                    .Select(nameof(Config.DatabaseVersion))
                    .OrderBy(nameof(Config.Id))
                );
            }
		    catch
		    {
		        // ignored
		    }

		    return databaseVersion;
		}

		public async Task<Config> GetConfigAsync()
        {
            return await _repository.GetAsync(Q.OrderBy(nameof(Config.Id)));
        }
    }
}
