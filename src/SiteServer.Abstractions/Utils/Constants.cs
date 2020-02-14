using System;
using System.Collections.Generic;
using System.Text;


namespace SiteServer.Abstractions
{
    public static class Constants
    {
        public const string ConfigFileName = "ss.json";
        public const string DefaultLanguage = "en";

        public const string AuthKeyUserHeader = "X-SS-USER-TOKEN";
        public const string AuthKeyUserCookie = "SS-USER-TOKEN";
        public const string AuthKeyUserQuery = "userToken";
        public const string AuthKeyAdminHeader = "X-SS-ADMIN-TOKEN";
        public const string AuthKeyAdminCookie = "SS-ADMIN-TOKEN";
        public const string AuthKeyAdminQuery = "adminToken";
        public const string AuthKeyApiHeader = "X-SS-API-KEY";
        public const string AuthKeyApiCookie = "SS-API-KEY";
        public const string AuthKeyApiQuery = "apiKey";
        public const int AccessTokenExpireDays = 7;

        public const string ScopeChannels = "Channels";
        public const string ScopeContents = "Contents";
        public const string ScopeAdministrators = "Administrators";
        public const string ScopeUsers = "Users";
        public const string ScopeStl = "STL";

        public static readonly List<string> ScopeList = new List<string>
        {
            ScopeChannels,
            ScopeContents,
            ScopeAdministrators,
            ScopeUsers,
            ScopeStl
        };

        public static class TopMenu
        {
            public const string SiteCurrent = "SiteCurrent";
            public const string SiteAll = "SiteAll";
            public const string IdSite = "Site";
            public const string IdPlugins = "Plugins";
        }

        public static class AppPermissions
        {
            public const string PluginsAdd = "plugins_add";
            public const string PluginsManagement = "plugins_management";

            public const string SettingsSitesAdd = "settings_sitesAdd";
            public const string SettingsSites = "settings_sites";
            public const string SettingsSitesUrl = "settings_sitesUrl";
            public const string SettingsSitesTables = "settings_sitesTables";
            public const string SettingsSitesTemplates = "settings_sitesTemplates";
            public const string SettingsSitesTemplatesOnline = "settings_sitesTemplatesOnline";

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
            public const string ChannelsTranslate = "site_channelsTranslate";
            public const string ContentsCheck = "site_contentsCheck";
            public const string Library = "site_library";
            public const string ContentsRecycle = "site_contentsRecycle";
            public const string Templates = "site_templates";
            public const string Specials = "site_specials";
            public const string TemplateMatch = "site_templateMatch";
            public const string TemplateAssets = "site_templateAssets";
            public const string TemplatePreview = "site_templatePreview";
            public const string TemplateReference = "site_templateReference";
            public const string ConfigSite = "site_configSite";
            public const string ConfigAttributes = "site_configAttributes";
            public const string ConfigContents = "site_configContents";
            public const string ConfigGroups = "site_configGroups";
            public const string ConfigTableStyles = "site_configTableStyles";
            public const string ConfigUpload = "site_configUpload";
            public const string ConfigWaterMark = "site_configWaterMark";
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
            public const string ContentCheckLevel1 = "cms_contentCheckLevel1";
            public const string ContentCheckLevel2 = "cms_contentCheckLevel2";
            public const string ContentCheckLevel3 = "cms_contentCheckLevel3";
            public const string ContentCheckLevel4 = "cms_contentCheckLevel4";
            public const string ContentCheckLevel5 = "cms_contentCheckLevel5";
        }

        public static DateTime SqlMinValue { get; } = new DateTime(1754, 1, 1, 0, 0, 0, 0);

        public static Encoding Gb2312 { get; } = Encoding.GetEncoding("gb2312");

        public const char PageSeparatorChar = '/';

        public const string AdminLogin = "后台管理员登录";

        public const string Unauthorized = "权限不足，访问被禁止";
        public const string NotFound = "请求的资源不存在";

        public static string GetSessionIdCacheKey(int userId)
        {
            return $"SESSION-ID-{userId}";
        }

        public const string ReturnAndNewline = "\r\n";//回车换行
        public const string Html5Empty = @"<html><head><meta charset=""utf-8""></head><body></body></html>";

        public const string Ellipsis = "...";

        public const int PageSize = 25;//后台分页数
        public const string HideElementStyle = "display:none";
        public const string ShowElementStyle = "display:";

        public const string TitleImageAppendix = "t_";
        public const string SmallImageAppendix = "s_";
    }
}
