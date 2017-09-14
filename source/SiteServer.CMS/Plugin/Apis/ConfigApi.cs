using System;
using BaiRong.Core;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
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
                        DataProvider.PluginConfigDao.Update(_metadata.Id, publishmentSystemId, name, json);
                    }
                    else
                    {
                        DataProvider.PluginConfigDao.Insert(_metadata.Id, publishmentSystemId, name, json);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex, $"插件： {_metadata.Name}");
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
                LogUtils.AddErrorLog(ex, $"插件： {_metadata.Name}");
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
                LogUtils.AddErrorLog(ex, $"插件： {_metadata.Name}");
                return false;
            }
            return true;
        }

        public bool SetGlobalConfig(object config)
        {
            return SetConfig(0, string.Empty, config);
        }

        public bool SetGlobalConfig(string name, object config)
        {
            return SetConfig(0, name, config);
        }

        public T GetGlobalConfig<T>(string name = "")
        {
            if (name == null) name = string.Empty;
            return GetConfig<T>(0, name);
        }

        public bool RemoveGlobalConfig(string name)
        {
            return RemoveConfig(0, name);
        }

        public string EncryptStringBySecretKey(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString);
        }

        public string DecryptStringBySecretKey(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString);
        }

        public string PhysicalApplicationPath => WebConfigUtils.PhysicalApplicationPath;

        public string AdminDirectory => WebConfigUtils.AdminDirectory;
    }
}
