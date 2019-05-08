using System;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;

namespace SiteServer.CMS.Apis
{
    public class ConfigApi : IConfigApi
    {
        private ConfigApi() { }

        private static ConfigApi _instance;
        public static ConfigApi Instance => _instance ?? (_instance = new ConfigApi());

        public bool SetConfig(string pluginId, int siteId, object config)
        {
            return SetConfig(pluginId, siteId, string.Empty, config);
        }

        public bool SetConfig(string pluginId, int siteId, string name, object config)
        {
            if (name == null) name = string.Empty;

            try
            {
                if (config == null)
                {
                    DataProvider.PluginConfig.Delete(pluginId, siteId, name);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                    if (DataProvider.PluginConfig.IsExists(pluginId, siteId, name))
                    {
                        var configInfo = new PluginConfigInfo
                        {
                            PluginId = pluginId,
                            SiteId = siteId,
                            ConfigName = name,
                            ConfigValue = json
                        };
                        DataProvider.PluginConfig.Update(configInfo);
                    }
                    else
                    {
                        var configInfo = new PluginConfigInfo
                        {
                            PluginId = pluginId,
                            SiteId = siteId,
                            ConfigName = name,
                            ConfigValue = json
                        };
                        DataProvider.PluginConfig.Insert(configInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return false;
            }
            return true;
        }

        public T GetConfig<T>(string pluginId, int siteId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                var value = DataProvider.PluginConfig.GetValue(pluginId, siteId, name);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
            }
            return default(T);
        }

        public bool RemoveConfig(string pluginId, int siteId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                DataProvider.PluginConfig.Delete(pluginId, siteId, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return false;
            }
            return true;
        }
    }
}
