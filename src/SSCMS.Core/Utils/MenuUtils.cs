namespace SSCMS.Core.Utils
{
    public static class MenuUtils
    {
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
            public const string SettingsHomeConfig = "app_settings_homeConfig";
            public const string SettingsHomeMenus = "app_settings_homeMenus";
            public const string SettingsAnalysisAdminLogin = "app_settings_analysisAdminLogin";
            public const string SettingsAnalysisSiteContent = "app_settings_analysisSiteContent";
            public const string SettingsAnalysisUser = "app_settings_analysisUser";
            public const string SettingsLogsSite = "app_settings_logsSite";
            public const string SettingsLogsAdmin = "app_settings_logsAdmin";
            public const string SettingsLogsUser = "app_settings_logsUser";
            public const string SettingsLogsError = "app_settings_logsError";
            public const string SettingsLogsConfig = "app_settings_logsConfig";
            public const string SettingsUtilitiesCache = "app_settings_utilitiesCache";
            public const string SettingsUtilitiesParameters = "app_settings_utilitiesParameters";
            public const string SettingsUtilitiesEncrypt = "app_settings_utilitiesEncrypt";
        }

        public static class SitePermissions
        {
            public const string Contents = "site_contents";
            public const string Channels = "site_channels";
            public const string ContentsSearch = "site_contentsSearch";
            public const string ContentsCheck = "site_contentsCheck";
            public const string MaterialMessage = "site_materialMessage";
            public const string MaterialImage = "site_materialImage";
            public const string MaterialVideo = "site_materialVideo";
            public const string MaterialAudio = "site_materialAudio";
            public const string MaterialFile = "site_materialFile";
            public const string ChannelsTranslate = "site_channelsTranslate";
            public const string ContentsRecycle = "site_contentsRecycle";
            public const string Templates = "site_templates";
            public const string Specials = "site_specials";
            public const string TemplatesMatch = "site_templatesMatch";
            public const string TemplatesIncludes = "site_templatesIncludes";
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
            public const string SettingsUploadAudio = "site_settingsUploadAudio";
            public const string SettingsUploadFile = "site_settingsUploadFile";
            public const string SettingsWaterMark = "site_settingsWaterMark";
            public const string CreateIndex = "site_createIndex";
            public const string CreateChannels = "site_createChannels";
            public const string CreateContents = "site_createContents";
            public const string CreateFiles = "site_createFiles";
            public const string CreateSpecials = "site_createSpecials";
            public const string CreateAll = "site_createAll";
            public const string CreateStatus = "site_createStatus";

            public const string WxAccount = "site_wxAccount";
            public const string WxReply = "site_wxReply";
            public const string WxReplyAuto = "site_wxReplyAuto";
            public const string WxReplyBeAdded = "site_wxReplyBeAdded";
            public const string WxMenus = "site_wxMenus";
            public const string WxChat = "site_wxChat";
            public const string WxUsers = "site_wxUsers";
            public const string WxSend = "site_wxSend";
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
    }
}
