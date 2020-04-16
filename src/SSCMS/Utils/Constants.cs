using System;
using System.Collections.Generic;

namespace SSCMS.Utils
{
    public static class Constants
    {
        public const string ConfigFileName = "sscms.json";
        public const string PluginPackageFileName = "package.json";
        public const string PluginConfigFileName = "config.json";
        public const string AdminDirectory = "ss-admin";
        public const string HomeDirectory = "home";
        public const string PluginsDirectory = "plugins";
        public const string DefaultLanguage = "en";
        public const string DefaultLocalDbFileName = "database.sqlite";
        public const string EncryptStingIndicator = "0secret0";

        public const int AccessTokenExpireDays = 7;

        public const string RoleTypeAdministrator = "Administrator";
        public const string RoleTypeUser = "User";
        public const string RoleTypeApi = "Api";

        public const string ScopeChannels = "Channels";
        public const string ScopeContents = "Contents";
        public const string ScopeAdministrators = "Administrators";
        public const string ScopeUsers = "Users";
        public const string ScopeStl = "STL";

        public const string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页占位符

        public const string ApiPrefix = "/api";
        public const string ApiAdminPrefix = "/api/admin";
        public const string ApiHomePrefix = "/api/home";
        public const string ApiV1Prefix = "/api/v1";
        public const string RoutePreview = "preview/{siteId}";
        public const string RoutePreviewChannel = "preview/{siteId}/{channelId}";
        public const string RoutePreviewContent = "preview/{siteId}/{channelId}/{contentId}";
        public const string RoutePreviewFile = "preview/{siteId}/file/{fileTemplateId}";
        public const string RoutePreviewSpecial = "preview/{siteId}/special/{specialId}";
        public const string RouteActionsDownload = "stl/actions/download";
        public const string RouteActionsDynamic = "sys/stl/actions/dynamic";
        public const string RouteRouteActionsIf = "sys/stl/actions/if";
        public const string RouteActionsPageContents = "sys/stl/actions/pagecontents";
        public const string RouteActionsSearch = "sys/stl/actions/search";
        public const string RouteActionsTrigger = "sys/stl/actions/trigger";

        public const string PackageId = "SS.CMS.App";

        public static readonly List<string> ScopeList = new List<string>
        {
            ScopeChannels,
            ScopeContents,
            ScopeAdministrators,
            ScopeUsers,
            ScopeStl
        };

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

            public const string SettingsAdministrators = "settings_administrators";
            public const string SettingsAdministratorsRole = "settings_administratorsRole";
            public const string SettingsAdministratorsConfig = "settings_administratorsConfig";
            public const string SettingsAdministratorsAccessTokens = "settings_administratorsAccessTokens";
            public const string SettingsUsers = "settings_users";
            public const string SettingsUsersGroup = "settings_usersGroup";
            public const string SettingsUsersStyle = "settings_usersStyle";
            public const string SettingsUsersConfig = "settings_usersConfig";
            public const string SettingsAnalysisAdminLogin = "settings_analysisAdminLogin";
            public const string SettingsAnalysisAdminWork = "settings_analysisAdminWork";
            public const string SettingsAnalysisUser = "settings_analysisUser";
            public const string SettingsLogsSite = "settings_logsSite";
            public const string SettingsLogsAdmin = "settings_logsAdmin";
            public const string SettingsLogsUser = "settings_logsUser";
            public const string SettingsLogsError = "settings_logsError";
            public const string SettingsLogsConfig = "settings_logsConfig";
            public const string SettingsConfigsAdmin = "settings_configsAdmin";
            public const string SettingsConfigsHome = "settings_configsHome";
            public const string SettingsConfigsHomeMenu = "settings_configsHomeMenu";
            public const string SettingsUtilitiesCache = "settings_utilitiesCache";
            public const string SettingsUtilitiesParameters = "settings_utilitiesParameters";
            public const string SettingsUtilitiesEncrypt = "settings_utilitiesEncrypt";
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
            public const string Add = "channel_add";
            public const string Edit = "channel_edit";
            public const string Delete = "channel_delete";
            public const string Translate = "channel_translate";
            public const string Create = "channel_create";
        }

        public static class ContentPermissions
        {
            public const string View = "content_view";
            public const string Add = "content_add";
            public const string Edit = "content_edit";
            public const string Delete = "content_delete";
            public const string Translate = "content_translate";
            public const string Arrange = "content_arrange";
            public const string CheckLevel1 = "content_checkLevel1";
            public const string CheckLevel2 = "content_checkLevel2";
            public const string CheckLevel3 = "content_checkLevel3";
            public const string CheckLevel4 = "content_checkLevel4";
            public const string CheckLevel5 = "content_checkLevel5";
            public const string Create = "content_create";
        }

        public static DateTime SqlMinValue { get; } = new DateTime(1754, 1, 1, 0, 0, 0, 0);

        public const string AdminLogin = "后台管理员登录";

        public static string GetSessionIdCacheKey(int userId)
        {
            return $"SESSION-ID-{userId}";
        }

        public const string ReturnAndNewline = "\r\n";//回车换行
        public const string Html5Empty = @"<html><head><meta charset=""utf-8""><meta http-equiv=""cache-control"" content=""max-age=0"" /><meta http-equiv=""cache-control"" content=""no-cache"" /><meta http-equiv=""expires"" content=""0"" /><meta http-equiv=""expires"" content=""Tue, 01 Jan 1980 1:00:00 GMT"" /><meta http-equiv=""pragma"" content=""no-cache"" /></head><body></body></html>";

        public const string Ellipsis = "...";

        public const int PageSize = 25;//后台分页数
        public const string SmallImageAppendix = "s_";
        public const string TitleImageAppendix = "t_";
    }
}
