using System;
using System.Collections.Generic;
using System.Linq;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin
{
    public class PublicApiInstance : IPublicApi
    {
        private readonly PluginMetadata _metadata;

        public PublicApiInstance(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        public bool SetOption(string option, string value)
        {
            try
            {
                ConfigManager.SystemConfigInfo.SetExtendedAttribute(option, value);
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                return false;
            }
            return true;
        }

        public string GetOption(string option)
        {
            try
            {
                return ConfigManager.SystemConfigInfo.GetExtendedAttribute(option);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
            return null;
        }

        public bool RemoveOption(string option)
        {
            try
            {
                ConfigManager.SystemConfigInfo.RemoveExtendedAttribute(option);
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                return false;
            }
            return true;
        }

        public bool SetSiteOption(int siteId, string option, string value)
        {
            try
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (publishmentSystemInfo == null) return false;

                publishmentSystemInfo.Additional.SetExtendedAttribute(option, value);
                DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                return false;
            }
            return true;
        }

        public string GetSiteOption(int siteId, string option)
        {
            try
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                return publishmentSystemInfo?.Additional.GetExtendedAttribute(option);
            }
            catch(Exception ex)
            {
                AddErrorLog(ex);
            }
            return null;
        }

        public bool RemoveSiteOption(int siteId, string option)
        {
            try
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (publishmentSystemInfo == null) return false;
                publishmentSystemInfo.Additional.RemoveExtendedAttribute(option);
                DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                return false;
            }
            return true;
        }

        public int GetSiteIdByFilePath(string path)
        {
            var publishmentSystemInfo = PathUtility.GetPublishmentSystemInfo(path);
            if (publishmentSystemInfo == null) return 0;

            return publishmentSystemInfo.PublishmentSystemId;
        }

        public string GetSiteDirectoryPath(int siteId)
        {
            if (siteId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
            if (publishmentSystemInfo == null) return null;

            return PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
        }

        public List<PluginPair> GetAllPlugins()
        {
            return PluginManager.AllPlugins.ToList();
        }

        public void AddErrorLog(Exception ex)
        {
            LogUtils.AddErrorLog(ex, $"插件： {_metadata.Name}");
        }
    }
}
