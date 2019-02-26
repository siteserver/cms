using System.Linq;

namespace SiteServer.Utils
{
    public static class AdminPagesUtils
    {
        public static string DashboardUrl => PageUtils.GetAdminUrl("dashboard.cshtml");
        public static string ErrorUrl => PageUtils.GetAdminUrl("error.cshtml");
        public static string MainUrl => PageUtils.GetAdminUrl("main.cshtml");
        public static string LoginUrl => PageUtils.GetAdminUrl("login.cshtml");
        public static string LogoutUrl => PageUtils.GetAdminUrl("logout.cshtml");
        public static string UpdateUrl => PageUtils.GetAdminUrl("update.cshtml");
        public static string UpgradeUrl => PageUtils.GetAdminUrl("upgrade.cshtml");
        public static string LoadingUrl => PageUtils.GetAdminUrl("loading.cshtml");

        public static class Plugins
        {
            private const string DirectoryName = nameof(Plugins);

            public static string ManageUrl => PageUtils.GetAdminUrl(PageUtils.Combine(DirectoryName, "manage.cshtml"));
        }

        public static class Settings
        {
            private const string DirectoryName = nameof(Settings);

            public static string AdministratorsUrl => PageUtils.GetAdminUrl(PageUtils.Combine(DirectoryName, "administrators.cshtml"));

            public static string SiteAddUrl => PageUtils.GetAdminUrl(PageUtils.Combine(DirectoryName, "siteAdd.cshtml"));
        }

        public static class Cms
        {
            private const string DirectoryName = nameof(Cms);

            private static string GetUrl(string pageName, int siteId, object param = null)
            {
                var url = PageUtils.GetAdminUrl(PageUtils.Combine(DirectoryName, $"{pageName}?siteId={siteId}"));
                return param == null ? url : param.GetType().GetProperties().Aggregate(url, (current, p) => current + $"&{p.Name.ToCamelCase()}={p.GetValue(param)}");
            }

            public static string GetContentsUrl(int siteId, int channelId)
            {
                return GetUrl("contents.cshtml", siteId, new
                {
                    channelId
                });
            }

            public static string GetCreateStatusUrl(int siteId)
            {
                return GetUrl("createStatus.cshtml", siteId);
            }
        }
    }
}
