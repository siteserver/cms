using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Services
{
    public partial class PluginManager
    {
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
                    await _pluginConfigRepository.DeleteAsync(pluginId, siteId, name);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                    if (await _pluginConfigRepository.IsExistsAsync(pluginId, siteId, name))
                    {
                        var pluginConfig = new PluginConfig
                        {
                            PluginId = pluginId,
                            SiteId = siteId,
                            ConfigName = name,
                            ConfigValue = json
                        };
                        await _pluginConfigRepository.UpdateAsync(pluginConfig);
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
                        await _pluginConfigRepository.InsertAsync(pluginConfig);
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
                var value = await _pluginConfigRepository.GetValueAsync(pluginId, siteId, name);
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
                await _pluginConfigRepository.DeleteAsync(pluginId, siteId, name);
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
