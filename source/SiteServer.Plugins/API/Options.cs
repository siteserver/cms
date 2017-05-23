using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.Plugins.API
{
    public sealed class Options
    {
        public static bool SetOption(string option, string value)
        {
            try
            {
                ConfigManager.SystemConfigInfo.SetExtendedAttribute(option, value);
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string GetOption(string option)
        {
            try
            {
                return ConfigManager.SystemConfigInfo.GetExtendedAttribute(option);
            }
            catch
            {
                // ignored
            }
            return null;
        }

        public static bool RemoveOption(string option)
        {
            try
            {
                ConfigManager.SystemConfigInfo.RemoveExtendedAttribute(option);
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool SetSiteOption(int siteId, string option, string value)
        {
            try
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (publishmentSystemInfo == null) return false;
                
                publishmentSystemInfo.Additional.SetExtendedAttribute(option, value);
                DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string GetSiteOption(int siteId, string option)
        {
            try
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                return publishmentSystemInfo?.Additional.GetExtendedAttribute(option);
            }
            catch
            {
                // ignored
            }
            return null;
        }

        public static bool RemoveSiteOption(int siteId, string option)
        {
            try
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (publishmentSystemInfo == null) return false;
                publishmentSystemInfo.Additional.RemoveExtendedAttribute(option);
                DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
