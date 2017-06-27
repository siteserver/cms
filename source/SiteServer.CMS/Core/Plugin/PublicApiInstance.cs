using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core.Plugin
{
    public class PublicApiInstance : IPublicApi
    {
        private readonly PluginMetadata _metadata;

        public PublicApiInstance(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        public string DatabaseType => EDatabaseTypeUtils.GetValue(WebConfigUtils.DatabaseType);

        public string ConnectionString => WebConfigUtils.ConnectionString;

        public IDbHelper DbHelper => WebConfigUtils.Helper;

        public int GetSiteIdByFilePath(string path)
        {
            var publishmentSystemInfo = PathUtility.GetPublishmentSystemInfo(path);
            return publishmentSystemInfo?.PublishmentSystemId ?? 0;
        }

        public string GetSiteDirectoryPath(int siteId)
        {
            if (siteId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
            return publishmentSystemInfo == null ? null : PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
        }

        public void AddErrorLog(Exception ex)
        {
            LogUtils.AddErrorLog(ex, $"插件： {_metadata.Name}");
        }

        public List<int> GetSiteIds()
        {
            return PublishmentSystemManager.GetPublishmentSystemIdList();
        }

        public bool SetSiteConfig(int siteId, string name, object config)
        {
            if (string.IsNullOrEmpty(name)) return false;

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
                        DataProvider.PluginConfigDao.Update(_metadata.Id, siteId, name, json);
                    }
                    else
                    {
                        DataProvider.PluginConfigDao.Insert(_metadata.Id, siteId, name, json);
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                return false;
            }
            return true;
        }

        public T GetSiteConfig<T>(int siteId, string name)
        {
            if (string.IsNullOrEmpty(name)) return default(T);

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
                AddErrorLog(ex);
            }
            return default(T);
        }

        public bool RemoveSiteConfig(int siteId, string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            try
            {
                DataProvider.PluginConfigDao.Delete(_metadata.Id, siteId, name);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                return false;
            }
            return true;
        }

        public bool SetConfig(string name, object config)
        {
            return SetSiteConfig(0, name, config);
        }

        public T GetConfig<T>(string name)
        {
            return GetSiteConfig<T>(0, name);
        }

        public bool RemoveConfig(string name)
        {
            return RemoveSiteConfig(0, name);
        }
    }
}
