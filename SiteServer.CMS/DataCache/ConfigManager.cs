using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class ConfigManager
    {
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(ConfigManager));
        private static readonly object LockObject = new object();

        public static class PluginsPermissions
        {
            public const string Add = "plugins_add";
            public const string Management = "plugins_management";
        }

        public static class SettingsPermissions
        {
            public const string SiteAdd = "settings_siteAdd";
            public const string Site = "settings_site";
            public const string Admin = "settings_admin";
            public const string User = "settings_user";
            public const string Chart = "settings_chart";
            public const string Log = "settings_log";
            public const string Utility = "settings_utility";
        }

        public static class WebSitePermissions
        {
            public const string Content = "cms_content";                            //信息管理
            public const string Template = "cms_template";                          //显示管理
            public const string Configration = "cms_configration";                  //设置管理
            public const string Create = "cms_create";                              //生成管理
        }

        public static class ChannelPermissions
        {
            public const string ContentView = "cms_contentView";
            public const string ContentAdd = "cms_contentAdd";
            public const string ContentEdit = "cms_contentEdit";
            public const string ContentDelete = "cms_contentDelete";
            public const string ContentTranslate = "cms_contentTranslate";
            public const string ContentOrder = "cms_contentOrder";
            public const string ChannelAdd = "cms_channelAdd";
            public const string ChannelEdit = "cms_channelEdit";
            public const string ChannelDelete = "cms_channelDelete";
            public const string ChannelTranslate = "cms_channelTranslate";
            public const string CreatePage = "cms_createPage";
            public const string ContentCheck = "cms_contentCheck";
            public const string ContentCheckLevel1 = "cms_contentCheckLevel1";
            public const string ContentCheckLevel2 = "cms_contentCheckLevel2";
            public const string ContentCheckLevel3 = "cms_contentCheckLevel3";
            public const string ContentCheckLevel4 = "cms_contentCheckLevel4";
            public const string ContentCheckLevel5 = "cms_contentCheckLevel5";
        }

        public static class TopMenu
        {
            public const string IdSite = "Site";
            public const string IdPlugins = "Plugins";
            public const string IdSettings = "Settings";
        }

        public static class LeftMenu
        {
            public const string IdContent = "Content";
            public const string IdTemplate = "Template";
            public const string IdConfigration = "Configration";
            public const string IdCreate = "Create";
        }

        public static string GetTopMenuName(string menuId)
        {
            var retval = string.Empty;
            if (menuId == TopMenu.IdSite)
            {
                retval = "站点管理";
            }
            else if (menuId == TopMenu.IdPlugins)
            {
                retval = "插件管理";
            }
            else if (menuId == TopMenu.IdSettings)
            {
                retval = "系统管理";
            }
            return retval;
        }

        public static string GetLeftMenuName(string menuId)
        {
            var retval = string.Empty;
            if (menuId == LeftMenu.IdContent)
            {
                retval = "信息管理";
            }
            else if (menuId == LeftMenu.IdTemplate)
            {
                retval = "显示管理";
            }
            else if (menuId == LeftMenu.IdConfigration)
            {
                retval = "设置管理";
            }
            else if (menuId == LeftMenu.IdCreate)
            {
                retval = "生成管理";
            }
            return retval;
        }

        public static ConfigInfo Instance
        {
            get
            {
                var retval = DataCacheManager.Get<ConfigInfo>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = DataCacheManager.Get<ConfigInfo>(CacheKey);
                    if (retval == null)
                    {
                        retval = DataProvider.ConfigDao.GetConfigInfo();
                        DataCacheManager.Insert(CacheKey, retval);
                    }
                }

                return retval;
            }
        }

        public static bool IsChanged
        {
            set
            {
                if (value)
                {
                    DataCacheManager.Remove(CacheKey);
                }
            }
        }

        public static SystemConfigInfo SystemConfigInfo => Instance.SystemConfigInfo;
    }
}
