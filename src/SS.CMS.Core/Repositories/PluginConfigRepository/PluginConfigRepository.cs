using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class PluginConfigRepository : IPluginConfigRepository
    {
        private readonly Repository<PluginConfigInfo> _repository;

        public PluginConfigRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<PluginConfigInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;


        private static class Attr
        {
            public const string PluginId = nameof(PluginConfigInfo.PluginId);
            public const string SiteId = nameof(PluginConfigInfo.SiteId);
            public const string ConfigName = nameof(PluginConfigInfo.ConfigName);
            public const string ConfigValue = nameof(PluginConfigInfo.ConfigValue);
        }

        public async Task<int> InsertAsync(PluginConfigInfo configInfo)
        {
            return await _repository.InsertAsync(configInfo);
        }

        public async Task DeleteAsync(string pluginId, int siteId, string configName)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.PluginId, pluginId)
                .Where(Attr.ConfigName, configName));
        }

        public async Task UpdateAsync(PluginConfigInfo configInfo)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.ConfigValue, configInfo.ConfigValue)
                .Where(Attr.PluginId, configInfo.PluginId)
                .Where(Attr.SiteId, configInfo.SiteId)
                .Where(Attr.ConfigName, configInfo.ConfigName)
            );
        }

        public async Task<string> GetValueAsync(string pluginId, int siteId, string configName)
        {
            return await _repository.GetAsync<string>(Q
                .Select(Attr.ConfigValue)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.PluginId, pluginId)
                .Where(Attr.ConfigName, configName));
        }

        public async Task<bool> IsExistsAsync(string pluginId, int siteId, string configName)
        {
            return await _repository.ExistsAsync(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.PluginId, pluginId)
                .Where(Attr.ConfigName, configName));
        }
    }
}
