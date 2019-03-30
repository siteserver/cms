using System.Linq;
using SiteServer.Utils;

namespace SiteServer.CMS.Fx
{
    public static class AdminPagesUtils
    {
        public static string DashboardUrl => FxUtils.GetAdminUrl("dashboard.cshtml");
        public static string ErrorUrl => FxUtils.GetAdminUrl("error.cshtml");
        public static string MainUrl => FxUtils.GetAdminUrl("main.cshtml");
        public static string LoginUrl => FxUtils.GetAdminUrl("login.cshtml");
        public static string LogoutUrl => FxUtils.GetAdminUrl("logout.cshtml");
        public static string UpdateUrl => FxUtils.GetAdminUrl("update.cshtml");
        public static string UpgradeUrl => FxUtils.GetAdminUrl("upgrade.cshtml");
        public static string LoadingUrl => FxUtils.GetAdminUrl("loading.cshtml");

        public static class Plugins
        {
            private const string DirectoryName = nameof(Plugins);

            public static string ManageUrl => FxUtils.GetAdminUrl(PageUtils.Combine(DirectoryName, "manage.cshtml"));
        }

        public static class Settings
        {
            private const string DirectoryName = nameof(Settings);

            public static string AdministratorsUrl => FxUtils.GetAdminUrl(PageUtils.Combine(DirectoryName, "administrators.cshtml"));

            public static string SiteAddUrl => FxUtils.GetAdminUrl(PageUtils.Combine(DirectoryName, "siteAdd.cshtml"));
        }

        public static class Cms
        {
            private const string DirectoryName = nameof(Cms);

            private static string GetUrl(string pageName, int siteId, object param = null)
            {
                var url = FxUtils.GetAdminUrl(PageUtils.Combine(DirectoryName, $"{pageName}?siteId={siteId}"));
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
