using System.Collections.Specialized;
using SS.CMS.Utils;

namespace SS.CMS.Core.Settings
{
    public static class AdminUrl
    {
        public static string IndexUrl => GetAdminUrl(string.Empty);

        public static string InstallUrl => GetAdminUrl("install");
        public static string DashboardUrl => GetAdminUrl("dashboard");
        public static string ErrorUrl => GetAdminUrl("error");
        public static string LoginUrl => GetAdminUrl("login");
        public static string SyncUrl => GetAdminUrl("sync");

        private static string GetAdminUrl(string relatedUrl)
        {
            return PageUtils.Combine(AppContext.ApplicationPath, relatedUrl);
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
    }
}
