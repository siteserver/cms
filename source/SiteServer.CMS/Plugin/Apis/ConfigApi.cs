using System;
using BaiRong.Core;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ConfigApi : IConfigApi
    {
        private readonly PluginMetadata _metadata;

        public ConfigApi(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        public bool SetConfig(int publishmentSystemId, object config)
        {
            return SetConfig(publishmentSystemId, string.Empty, config);
        }

        public bool SetConfig(int publishmentSystemId, string name, object config)
        {
            if (name == null) name = string.Empty;

            try
            {
                if (config == null)
                {
                    DataProvider.PluginConfigDao.Delete(_metadata.Id, publishmentSystemId, name);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                    if (DataProvider.PluginConfigDao.IsExists(_metadata.Id, publishmentSystemId, name))
                    {
                        var configInfo = new PluginConfigInfo(0, _metadata.Id, publishmentSystemId, name, json);
                        DataProvider.PluginConfigDao.Update(configInfo);
                    }
                    else
                    {
                        var configInfo = new PluginConfigInfo(0, _metadata.Id, publishmentSystemId, name, json);
                        DataProvider.PluginConfigDao.Insert(configInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(_metadata.Id, ex);
                return false;
            }
            return true;
        }

        public T GetConfig<T>(int publishmentSystemId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                var value = DataProvider.PluginConfigDao.GetValue(_metadata.Id, publishmentSystemId, name);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(_metadata.Id, ex);
            }
            return default(T);
        }

        public bool RemoveConfig(int publishmentSystemId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                DataProvider.PluginConfigDao.Delete(_metadata.Id, publishmentSystemId, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(_metadata.Id, ex);
                return false;
            }
            return true;
        }

        public string PhysicalApplicationPath => WebConfigUtils.PhysicalApplicationPath;

        public string AdminDirectory => WebConfigUtils.AdminDirectory;

        public ISystemConfigInfo SystemConfigInfo => ConfigManager.SystemConfigInfo;
    }
}
