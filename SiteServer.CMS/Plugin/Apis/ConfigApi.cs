using System;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ConfigApi : IConfigApi
    {
        private readonly IMetadata _metadata;

        public ConfigApi(IMetadata metadata)
        {
            _metadata = metadata;
        }

        public bool SetConfig(int siteId, object config)
        {
            return SetConfig(siteId, string.Empty, config);
        }

        public bool SetConfig(int siteId, string name, object config)
        {
            if (name == null) name = string.Empty;

            try
            {
                if (config == null)
                {
                    DataProvider.PluginConfigDao.Delete(_metadata.Id, siteId, name);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                    if (DataProvider.PluginConfigDao.IsExists(_metadata.Id, siteId, name))
                    {
                        var configInfo = new PluginConfigInfo(0, _metadata.Id, siteId, name, json);
                        DataProvider.PluginConfigDao.Update(configInfo);
                    }
                    else
                    {
                        var configInfo = new PluginConfigInfo(0, _metadata.Id, siteId, name, json);
                        DataProvider.PluginConfigDao.Insert(configInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(_metadata.Id, ex);
                return false;
            }
            return true;
        }

        public T GetConfig<T>(int siteId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                var value = DataProvider.PluginConfigDao.GetValue(_metadata.Id, siteId, name);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(_metadata.Id, ex);
            }
            return default(T);
        }

        public bool RemoveConfig(int siteId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                DataProvider.PluginConfigDao.Delete(_metadata.Id, siteId, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(_metadata.Id, ex);
                return false;
            }
            return true;
        }

        public IAttributes SystemConfig => ConfigManager.SystemConfigInfo;
    }
}
