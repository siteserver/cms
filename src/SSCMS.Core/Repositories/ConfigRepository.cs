using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public partial class ConfigRepository : IConfigRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<Config> _repository;
        private readonly string _cacheKey;

        public ConfigRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<Config>(settingsManager.Database, settingsManager.Redis);
            _cacheKey = CacheUtils.GetEntityKey(TableName);
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
                if (string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString)) return false;
                return await _repository.ExistsAsync();
            }
		    catch
		    {
		        // ignored
		    }

            return false;
        }

        public async Task UpdateConfigVersionAsync(string version)
        {
            var config = await GetAsync();
            if (config.Id == 0)
            {
                config = new Config
                {
                    Id = 0,
                    DatabaseVersion = version,
                    UpdateDate = DateTime.Now
                };
                config.Id = await InsertAsync(config);
            }
            else
            {
                config.DatabaseVersion = version;
                config.UpdateDate = DateTime.Now;
                await UpdateAsync(config);
            }
        }

        public async Task<bool> IsNeedInstallAsync()
        {
            var isNeedInstall = !await IsInitializedAsync();
            if (isNeedInstall)
            {
                isNeedInstall = !await IsInitializedAsync();
            }
            return isNeedInstall;
        }
    }
}
