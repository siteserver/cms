using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Newtonsoft.Json;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class PluginConfigRepository : IPluginConfigRepository
    {
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly Repository<PluginConfig> _repository;

        public PluginConfigRepository(ISettingsManager settingsManager, IErrorLogRepository errorLogRepository)
        {
            _repository = new Repository<PluginConfig>(settingsManager.Database, settingsManager.Redis);
            _errorLogRepository = errorLogRepository;
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

        public async Task<bool> SetConfigAsync(string pluginId, int siteId, object config)
        {
            return await SetConfigAsync(pluginId, siteId, string.Empty, config);
        }

        public async Task<bool> SetConfigAsync(string pluginId, int siteId, string name, object config)
        {
            if (name == null) name = string.Empty;

            try
            {
                if (config == null)
                {
                    await DeleteAsync(pluginId, siteId, name);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                    if (await IsExistsAsync(pluginId, siteId, name))
                    {
                        var pluginConfig = new PluginConfig
                        {
                            PluginId = pluginId,
                            SiteId = siteId,
                            ConfigName = name,
                            ConfigValue = json
                        };
                        await UpdateAsync(pluginConfig);
                    }
                    else
                    {
                        var pluginConfig = new PluginConfig
                        {
                            PluginId = pluginId,
                            SiteId = siteId,
                            ConfigName = name,
                            ConfigValue = json
                        };
                        await InsertAsync(pluginConfig);
                    }
                }
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex);
                return false;
            }
            return true;
        }

        public async Task<T> GetConfigAsync<T>(string pluginId, int siteId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                var value = await GetValueAsync(pluginId, siteId, name);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex);
            }
            return default(T);
        }

        public async Task<bool> RemoveConfigAsync(string pluginId, int siteId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                await DeleteAsync(pluginId, siteId, name);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex);
                return false;
            }
            return true;
        }
    }
}
