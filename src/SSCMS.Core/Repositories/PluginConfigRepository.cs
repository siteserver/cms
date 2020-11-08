using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Newtonsoft.Json;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

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

        private string GetCacheKey(string pluginId, int siteId, string name)
        {
            return CacheUtils.GetEntityKey(_repository.TableName, pluginId, $"{siteId}_{name}");
        }

        private string GetCacheKey(PluginConfig configInfo)
        {
            return GetCacheKey(configInfo.PluginId, configInfo.SiteId, configInfo.ConfigName);
        }

        private async Task InsertAsync(PluginConfig config)
        {
            await _repository.InsertAsync(config);
        }

        private async Task DeleteAsync(string pluginId, int siteId, string name)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(PluginConfig.SiteId), siteId)
                .Where(nameof(PluginConfig.PluginId), pluginId)
                .Where(nameof(PluginConfig.ConfigName), name)
                .CachingRemove(GetCacheKey(pluginId, siteId, name))
            );
        }

        private async Task UpdateAsync(PluginConfig configInfo)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(PluginConfig.ConfigValue), configInfo.ConfigValue)
                .Where(nameof(PluginConfig.PluginId), configInfo.PluginId)
                .Where(nameof(PluginConfig.SiteId), configInfo.SiteId)
                .Where(nameof(PluginConfig.ConfigName), configInfo.ConfigName)
                .CachingRemove(GetCacheKey(configInfo))
            );
        }

        private async Task<string> GetConfigValueAsync(string pluginId, int siteId, string name)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(PluginConfig.ConfigValue))
                .Where(nameof(PluginConfig.SiteId), siteId)
                .Where(nameof(PluginConfig.PluginId), pluginId)
                .Where(nameof(PluginConfig.ConfigName), name)
                .CachingGet(GetCacheKey(pluginId, siteId, name))
            );
        }

        private static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public async Task<T> GetAsync<T>(string pluginId, int siteId, string name)
        {
            if (name == null) name = string.Empty;

            try
            {
                var value = await GetConfigValueAsync(pluginId, siteId, name);
                var typeCode = Type.GetTypeCode(typeof(T));
                if (typeCode == TypeCode.Int32)
                {
                    return TranslateUtils.Get<T>(TranslateUtils.ToInt(value));
                }
                if (typeCode == TypeCode.Decimal)
                {
                    return TranslateUtils.Get<T>(TranslateUtils.ToDecimal(value));
                }
                if (typeCode == TypeCode.DateTime)
                {
                    return TranslateUtils.Get<T>(TranslateUtils.ToDateTime(value));
                }
                if (typeCode == TypeCode.Boolean)
                {
                    return TranslateUtils.Get<T>(TranslateUtils.ToBool(value));
                }
                if (typeCode == TypeCode.String)
                {
                    return TranslateUtils.Get<T>(value);
                }
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value, Settings);
                }
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex);
            }
            return default;
        }

        public async Task<T> GetAsync<T>(string pluginId, string name)
        {
            return await GetAsync<T>(pluginId, 0, name);
        }

        [Obsolete]
        public async Task<T> GetConfigAsync<T>(string pluginId, int siteId, string name)
        {
            return await GetAsync<T>(pluginId, siteId, name);
        }

        [Obsolete]
        public async Task<T> GetConfigAsync<T>(string pluginId, string name)
        {
            return await GetAsync<T>(pluginId, name);
        }

        public async Task<bool> SetAsync<T>(string pluginId, int siteId, string name, T value)
        {
            if (name == null) name = string.Empty;

            try
            {
                if (value == null)
                {
                    await DeleteAsync(pluginId, siteId, name);
                    return true;
                }

                string configValue;
                var typeCode = Type.GetTypeCode(typeof(T));
                if (typeCode == TypeCode.Int32 || typeCode == TypeCode.Decimal || typeCode == TypeCode.DateTime ||
                    typeCode == TypeCode.Boolean || typeCode == TypeCode.String)
                {
                    configValue = value.ToString();
                }
                else
                {
                    configValue = JsonConvert.SerializeObject(value, Settings);
                }

                if (await ExistsAsync(pluginId, siteId, name))
                {
                    var pluginConfig = new PluginConfig
                    {
                        PluginId = pluginId,
                        SiteId = siteId,
                        ConfigName = name,
                        ConfigValue = configValue
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
                        ConfigValue = configValue
                    };
                    await InsertAsync(pluginConfig);
                }
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex);
                return false;
            }

            return true;
        }

        public async Task<bool> SetAsync<T>(string pluginId, string name, T config)
        {
            return await SetAsync(pluginId, 0, name, config);
        }

        [Obsolete]
        public async Task<bool> SetConfigAsync<T>(string pluginId, int siteId, string name, T value)
        {
            return await SetAsync(pluginId, siteId, name, value);
        }

        [Obsolete]
        public async Task<bool> SetConfigAsync<T>(string pluginId, string name, T value)
        {
            return await SetAsync(pluginId, name, value);
        }

        public async Task<bool> ExistsAsync(string pluginId, int siteId, string name)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(PluginConfig.SiteId), siteId)
                .Where(nameof(PluginConfig.PluginId), pluginId)
                .Where(nameof(PluginConfig.ConfigName), name)
            );
        }

        public async Task<bool> ExistsAsync(string pluginId, string name)
        {
            return await ExistsAsync(pluginId, 0, name);
        }

        public async Task<bool> RemoveAsync(string pluginId, int siteId, string name)
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

        public async Task<bool> RemoveAsync(string pluginId, string name)
        {
            return await RemoveAsync(pluginId, 0, name);
        }
    }
}
