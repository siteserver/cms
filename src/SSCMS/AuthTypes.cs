namespace SSCMS
{
    public static class AuthTypes
    {
        public static class Claims
        {
            public const string UserId = System.Security.Claims.ClaimTypes.NameIdentifier;
            public const string UserName = System.Security.Claims.ClaimTypes.Name;
            public const string Role = System.Security.Claims.ClaimTypes.Role;
            public const string IsPersistent = System.Security.Claims.ClaimTypes.IsPersistent;
        }

        public static class Roles
        {
            public const string Administrator = nameof(Administrator);
            public const string User = nameof(User);
            public const string Api = nameof(Api);
        }

        public static class Resources
        {
            public const string App = "app";
            public const string Site = "site";
            public const string SiteChannel = "site:channel";
            public const string SiteContent = "site:content";
        }

        public static class AppPermissions
        {
            public const string PluginsAdd = "app_plugins_add";
            public const string PluginsManagement = "app_plugins_management";

            public const string SettingsSitesAdd = "app_settings_sitesAdd";
            public const string SettingsSites = "app_settings_sites";
            public const string SettingsSitesUrl = "app_settings_sitesUrl";
            public const string SettingsSitesTables = "app_settings_sitesTables";
            public const string SettingsSitesTemplates = "app_settings_sitesTemplates";
            public const string SettingsSitesTemplatesOnline = "app_settings_sitesTemplatesOnline";

            public const string SettingsAdministrators = "app_settings_administrators";
            public const string SettingsAdministratorsRole = "app_settings_administratorsRole";
            public const string SettingsAdministratorsConfig = "app_settings_administratorsConfig";
            public const string SettingsAdministratorsAccessTokens = "app_settings_administratorsAccessTokens";
            public const string SettingsUsers = "app_settings_users";
            public const string SettingsUsersGroup = "app_settings_usersGroup";
            public const string SettingsUsersStyle = "app_settings_usersStyle";
            public const string SettingsUsersConfig = "app_settings_usersConfig";
            public const string SettingsAnalysisAdminLogin = "app_settings_analysisAdminLogin";
            public const string SettingsAnalysisAdminWork = "app_settings_analysisAdminWork";
            public const string SettingsAnalysisUser = "app_settings_analysisUser";
            public const string SettingsLogsSite = "app_settings_logsSite";
            public const string SettingsLogsAdmin = "app_settings_logsAdmin";
            public const string SettingsLogsUser = "app_settings_logsUser";
            public const string SettingsLogsError = "app_settings_logsError";
            public const string SettingsLogsConfig = "app_settings_logsConfig";
            public const string SettingsConfigsAdmin = "app_settings_configsAdmin";
            public const string SettingsConfigsHome = "app_settings_configsHome";
            public const string SettingsConfigsHomeMenu = "app_settings_configsHomeMenu";
            public const string SettingsUtilitiesCache = "app_settings_utilitiesCache";
            public const string SettingsUtilitiesParameters = "app_settings_utilitiesParameters";
            public const string SettingsUtilitiesEncrypt = "app_settings_utilitiesEncrypt";
        }

        public static class SitePermissions
        {
            public const string Contents = "site_contents";
            public const string Channels = "site_channels";
            public const string ContentsSearch = "site_contentsSearch";
            public const string ChannelsTranslate = "site_channelsTranslate";
            public const string ContentsCheck = "site_contentsCheck";
            public const string LibraryText = "site_libraryText";
            public const string LibraryImage = "site_libraryImage";
            public const string LibraryVideo = "site_libraryVideo";
            public const string LibraryFile = "site_libraryFile";
            public const string ContentsRecycle = "site_contentsRecycle";
            public const string Templates = "site_templates";
            public const string Specials = "site_specials";
            public const string TemplatesMatch = "site_templatesMatch";
            public const string TemplatesAssets = "site_templatesAssets";
            public const string TemplatesPreview = "site_templatesPreview";
            public const string TemplatesReference = "site_templatesReference";
            public const string SettingsSite = "site_settingsSite";
            public const string SettingsContent = "site_settingsContent";
            public const string SettingsChannelGroup = "site_settingsChannelGroup";
            public const string SettingsContentGroup = "site_settingsContentGroup";
            public const string SettingsContentTag = "site_settingsContentTag";
            public const string SettingsStyleContent = "site_settingsStyleContent";
            public const string SettingsStyleChannel = "site_settingsStyleChannel";
            public const string SettingsStyleSite = "site_settingsStyleSite";
            public const string SettingsStyleRelatedField = "site_settingsStyleRelatedField";
            public const string SettingsCrossSiteTrans = "site_settingsCrossSiteTrans";
            public const string SettingsCrossSiteTransChannels = "site_settingsCrossSiteTransChannels";
            public const string SettingsCreateRule = "site_settingsCreateRule";
            public const string SettingsCreate = "site_settingsCreate";
            public const string SettingsCreateTrigger = "site_settingsCreateTrigger";
            public const string SettingsUploadImage = "site_settingsUploadImage";
            public const string SettingsUploadVideo = "site_settingsUploadVideo";
            public const string SettingsUploadFile = "site_settingsUploadFile";
            public const string SettingsWaterMark = "site_settingsWaterMark";
            public const string CreateIndex = "site_createIndex";
            public const string CreateChannels = "site_createChannels";
            public const string CreateContents = "site_createContents";
            public const string CreateFiles = "site_createFiles";
            public const string CreateSpecials = "site_createSpecials";
            public const string CreateAll = "site_createAll";
            public const string CreateStatus = "site_createStatus";
        }

        public static class SiteChannelPermissions
        {
            public const string Add = "site_channel_add";
            public const string Edit = "site_channel_edit";
            public const string Delete = "site_channel_delete";
            public const string Translate = "site_channel_translate";
            public const string Create = "site_channel_create";
        }

        public static class SiteContentPermissions
        {
            public const string View = "site_content_view";
            public const string Add = "site_content_add";
            public const string Edit = "site_content_edit";
            public const string Delete = "site_content_delete";
            public const string Translate = "site_content_translate";
            public const string Arrange = "site_content_arrange";
            public const string CheckLevel1 = "site_content_checkLevel1";
            public const string CheckLevel2 = "site_content_checkLevel2";
            public const string CheckLevel3 = "site_content_checkLevel3";
            public const string CheckLevel4 = "site_content_checkLevel4";
            public const string CheckLevel5 = "site_content_checkLevel5";
            public const string Create = "site_content_create";
        }
    }
}
