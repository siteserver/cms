using System;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;

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

        public static class AppPermissions
        {
            public const string PluginsAdd = "plugins_add";
            public const string PluginsManagement = "plugins_management";
            public const string SettingsSiteAdd = "settings_siteAdd";
            public const string SettingsSite = "settings_site";
            public const string SettingsSiteUrl = "settings_siteUrl";
            public const string SettingsSiteTables = "settings_siteTables";
            public const string SettingsSiteTemplates = "settings_siteTemplates";
            public const string SettingsSiteTemplatesOnline = "settings_siteTemplatesOnline";
            public const string SettingsAdmin = "settings_admin";
            public const string SettingsAdminRole = "settings_adminRole";
            public const string SettingsAdminConfig = "settings_adminConfig";
            public const string SettingsAdminAccessTokens = "settings_adminAccessTokens";
            public const string SettingsUser = "settings_user";
            public const string SettingsUserGroup = "settings_userGroup";
            public const string SettingsUserStyle = "settings_userStyle";
            public const string SettingsUserConfig = "settings_userConfig";
            public const string SettingsAnalysisSite = "settings_analysisSite";
            public const string SettingsAnalysisAdminLogin = "settings_analysisAdminLogin";
            public const string SettingsAnalysisAdminWork = "settings_analysisAdminWork";
            public const string SettingsAnalysisUser = "settings_analysisUser";
            public const string SettingsLogSite = "settings_logSite";
            public const string SettingsLogAdmin = "settings_logAdmin";
            public const string SettingsLogUser = "settings_logUser";
            public const string SettingsLogError = "settings_logError";
            public const string SettingsLogConfig = "settings_logConfig";
            public const string SettingsConfigAdmin = "settings_configAdmin";
            public const string SettingsConfigHome = "settings_configHome";
            public const string SettingsConfigHomeMenu = "settings_configHomeMenu";
            public const string SettingsUtilityCache = "settings_utilityCache";
            public const string SettingsUtilityParameters = "settings_utilityParameters";
            public const string SettingsUtilityEncrypt = "settings_utilityEncrypt";
            public const string SettingsUtilityDbLogDelete = "settings_utilityDbLogDelete";
        }

        public static class SitePermissions
        {
            public const string Contents = "site_contents";
            public const string Channels = "site_channels";
            public const string ContentsSearch = "site_contentsSearch";
            public const string ContentsWriting = "site_contentsWriting";
            public const string ContentsMy = "site_contentsMy";
            public const string ContentsTranslate = "site_contentsTranslate";
            public const string ContentsCheck = "site_contentsCheck";
            public const string ContentsTrash = "site_contentsTrash";
            public const string Library = "site_library";
            public const string Templates = "site_templates";
            public const string Specials = "site_specials";
            public const string TemplatesMatch = "site_templatesMatch";
            public const string TemplatesIncludes = "site_templatesIncludes";
            public const string TemplatesCss = "site_templatesCss";
            public const string TemplatesJs = "site_templatesJs";
            public const string TemplatesPreview = "site_templatesPreview";
            public const string TemplatesReference = "site_templatesReference";
            public const string ConfigSite = "site_configSite";
            public const string ConfigAttributes = "site_configAttributes";
            public const string ConfigContents = "site_configContents";
            public const string ConfigGroups = "site_configGroups";
            public const string ConfigTableStyles = "site_configTableStyles";
            public const string ConfigUpload = "site_configUpload";
            public const string ConfigCrossSiteTrans = "site_configCrossSiteTrans";
            public const string ConfigCreateRule = "site_configCreateRule";
            public const string CreateIndex = "site_createIndex";
            public const string CreateChannels = "site_createChannels";
            public const string CreateContents = "site_createContents";
            public const string CreateFiles = "site_createFiles";
            public const string CreateSpecials = "site_createSpecials";
            public const string CreateAll = "site_createAll";
            public const string CreateStatus = "site_createStatus";
        }

        public static class ChannelPermissions
        {
            public const string ContentView = "cms_contentView";
            public const string ContentAdd = "cms_contentAdd";
            public const string ContentEdit = "cms_contentEdit";
            public const string ContentDelete = "cms_contentDelete";
            public const string ContentTranslate = "cms_contentTranslate";
            public const string ContentArrange = "cms_contentArrange";
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
            var retVal = string.Empty;
            if (menuId == TopMenu.IdSite)
            {
                retVal = "站点管理";
            }
            else if (menuId == TopMenu.IdPlugins)
            {
                retVal = "插件管理";
            }
            else if (menuId == TopMenu.IdSettings)
            {
                retVal = "系统管理";
            }
            return retVal;
        }

        public static string GetLeftMenuName(string menuId)
        {
            var retVal = string.Empty;
            if (menuId == LeftMenu.IdContent)
            {
                retVal = "信息管理";
            }
            else if (menuId == LeftMenu.IdTemplate)
            {
                retVal = "显示管理";
            }
            else if (menuId == LeftMenu.IdConfigration)
            {
                retVal = "设置管理";
            }
            else if (menuId == LeftMenu.IdCreate)
            {
                retVal = "生成管理";
            }
            return retVal;
        }

        public static ConfigInfo Instance
        {
            get
            {
                var retVal = DataCacheManager.Get<ConfigInfo>(CacheKey);
                if (retVal != null) return retVal;

                lock (LockObject)
                {
                    retVal = DataCacheManager.Get<ConfigInfo>(CacheKey);
                    if (retVal == null)
                    {
                        try
                        {
                            retVal = DataProvider.ConfigDao.GetConfigInfo();
                            DataCacheManager.Insert(CacheKey, retVal);
                        }
                        catch
                        {
                            return new ConfigInfo(0, false, string.Empty, DateTime.Now, string.Empty);
                        }
                    }
                }

                return retVal;
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
