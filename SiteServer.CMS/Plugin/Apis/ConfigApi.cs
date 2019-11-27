using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ConfigApi : IConfigApi
    {
        private ConfigApi() { }

        private static ConfigApi _instance;
        public static ConfigApi Instance => _instance ??= new ConfigApi();

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
                    await DataProvider.PluginConfigDao.DeleteAsync(pluginId, siteId, name);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                    if (await DataProvider.PluginConfigDao.IsExistsAsync(pluginId, siteId, name))
                    {
                        var configInfo = new PluginConfig
                        {
                            Id = 0,
                            PluginId = pluginId,
                            SiteId = siteId,
                            ConfigName = name,
                            ConfigValue = json
                        };
                        await DataProvider.PluginConfigDao.UpdateAsync(configInfo);
                    }
                    else
                    {
                        var configInfo = new PluginConfig
                        {
                            Id = 0,
                            PluginId = pluginId,
                            SiteId = siteId,
                            ConfigName = name,
                            ConfigValue = json
                        };
                        await DataProvider.PluginConfigDao.InsertAsync(configInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(pluginId, ex);
                return false;
            }
            return true;
        }

        public async Task<T> GetConfigAsync<T>(string pluginId, int siteId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                var value = await DataProvider.PluginConfigDao.GetValueAsync(pluginId, siteId, name);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(pluginId, ex);
            }
            return default(T);
        }

        public async Task<bool> RemoveConfigAsync(string pluginId, int siteId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                await DataProvider.PluginConfigDao.DeleteAsync(pluginId, siteId, name);
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(pluginId, ex);
                return false;
            }
            return true;
        }
    }
}
