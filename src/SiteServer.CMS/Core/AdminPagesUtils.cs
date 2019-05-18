using System.Collections.Specialized;
using System.Linq;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class AdminPagesUtils
    {
        public static string IndexUrl => GetAdminUrl("");
        public static string DashboardUrl => GetAdminUrl("dashboard");
        public static string ErrorUrl => GetAdminUrl("error");
        public static string LoginUrl => GetAdminUrl("login");
        public static string LogoutUrl => GetAdminUrl("logout");
        public static string UpdateUrl => GetAdminUrl("update");
        public static string UpgradeUrl => GetAdminUrl("upgrade");
        public static string LoadingUrl => GetAdminUrl("loading");

        public static string GetAdminUrl(string relatedUrl)
        {
            return PageUtils.Combine(WebConfigUtils.ApplicationPath, relatedUrl);
        }

        public static string GetIndexUrl(int siteId, string pageUrl)
        {
            var queryString = new NameValueCollection();
            if (siteId > 0)
            {
                queryString.Add("siteId", siteId.ToString());
            }
            if (!string.IsNullOrEmpty(pageUrl))
            {
                queryString.Add("pageUrl", PageUtils.UrlEncode(pageUrl));
            }
            return PageUtils.AddQueryString(IndexUrl, queryString);
        }

        public static class Plugins
        {
            private const string DirectoryName = nameof(Plugins);

            public static string ManageUrl => GetAdminUrl(PageUtils.Combine(DirectoryName, "manage.cshtml"));
        }

        public static class Settings
        {
            private const string DirectoryName = nameof(Settings);

            public static string AdministratorsUrl => GetAdminUrl(PageUtils.Combine(DirectoryName, "administrators.cshtml"));

            public static string SiteAddUrl => GetAdminUrl(PageUtils.Combine(DirectoryName, "siteAdd.cshtml"));
        }

        public static class Cms
        {
            private const string DirectoryName = nameof(Cms);

            private static string GetUrl(string pageName, int siteId, object param = null)
            {
                var url = GetAdminUrl(PageUtils.Combine(DirectoryName, $"{pageName}?siteId={siteId}"));
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