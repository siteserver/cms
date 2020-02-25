namespace SS.CMS.Abstractions
{
    public static class AuthTypes
    {
        public static class ClaimTypes
        {
            public const string UserId = System.Security.Claims.ClaimTypes.NameIdentifier;
            public const string UserName = System.Security.Claims.ClaimTypes.Name;
            public const string Role = System.Security.Claims.ClaimTypes.Role;
        }

        public static class Roles
        {
            public const string Administrator = nameof(Administrator);

            public const string SiteAdministrator = nameof(SiteAdministrator);

            public const string SuperAdministrator = nameof(SuperAdministrator);
        }

        public static class Menus
        {
            public const string Sites = nameof(Sites);
        }

        public static class AppPermissions
        {
            public const string PluginsAdd = "plugins_add";
            public const string PluginsManagement = "plugins_management";
            public const string SettingsSiteAdd = "settings_siteAdd";
            public const string SettingsSite = "settings_site";
            public const string SettingsAdmin = "settings_admin";
            public const string SettingsUser = "settings_user";
            public const string SettingsChart = "settings_chart";
            public const string SettingsLog = "settings_log";
            public const string SettingsUtility = "settings_utility";
        }

        public static class SitePermissions
        {
            public const string Content = "cms_content";
            public const string Template = "cms_template";
            public const string Configuration = "cms_configuration";
            public const string Create = "cms_create";
        }

        public static string GetSiteAdministratorRoleName(int siteId)
        {
            return $"{siteId}:{Roles.SiteAdministrator}";
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
    }
}
