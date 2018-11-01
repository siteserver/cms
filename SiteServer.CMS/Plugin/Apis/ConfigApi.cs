using System;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
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
                    DataProvider.PluginConfigDao.Delete(pluginId, siteId, name);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                    if (DataProvider.PluginConfigDao.IsExists(pluginId, siteId, name))
                    {
                        var configInfo = new PluginConfigInfo(0, pluginId, siteId, name, json);
                        DataProvider.PluginConfigDao.Update(configInfo);
                    }
                    else
                    {
                        var configInfo = new PluginConfigInfo(0, pluginId, siteId, name, json);
                        DataProvider.PluginConfigDao.Insert(configInfo);
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
                var value = DataProvider.PluginConfigDao.GetValue(pluginId, siteId, name);
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
                DataProvider.PluginConfigDao.Delete(pluginId, siteId, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return false;
            }
            return true;
        }

        public IAttributes Config => ConfigManager.SystemConfigInfo;
    }
}
