using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS;

namespace SSCMS.Core.Repositories.PluginConfigRepository
{
    public class PluginConfigRepository : IPluginConfigRepository
    {
        private readonly Repository<PluginConfig> _repository;

        public PluginConfigRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<PluginConfig>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(PluginConfig config)
        {
            await _repository.InsertAsync(config);
        }

        public async Task DeleteAsync(string pluginId, int siteId, string configName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(PluginConfig.SiteId), siteId)
                .Where(nameof(PluginConfig.PluginId), pluginId)
                .Where(nameof(PluginConfig.ConfigName), configName)
            );
        }

        public async Task UpdateAsync(PluginConfig configInfo)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(PluginConfig.ConfigValue), configInfo.ConfigValue)
                .Where(nameof(PluginConfig.PluginId), configInfo.PluginId)
                .Where(nameof(PluginConfig.SiteId), configInfo.SiteId)
                .Where(nameof(PluginConfig.ConfigName), configInfo.ConfigName)
            );
        }

        public async Task<string> GetValueAsync(string pluginId, int siteId, string configName)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(PluginConfig.ConfigValue))
                .Where(nameof(PluginConfig.SiteId), siteId)
                .Where(nameof(PluginConfig.PluginId), pluginId)
                .Where(nameof(PluginConfig.ConfigName), configName)
            );
        }

        public async Task<bool> IsExistsAsync(string pluginId, int siteId, string configName)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(PluginConfig.SiteId), siteId)
                .Where(nameof(PluginConfig.PluginId), pluginId)
                .Where(nameof(PluginConfig.ConfigName), configName)
            );
        }
    }
}
