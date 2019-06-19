using System.Collections.Specialized;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UrlManager
    {
        public string AdminIndexUrl => GetAdminUrl(string.Empty);

        public string AdminInstallUrl => GetAdminUrl("install/");
        public string AdminDashboardUrl => GetAdminUrl("dashboard/");
        public string AdminErrorUrl => GetAdminUrl("error/");
        public string AdminLoginUrl => GetAdminUrl("login/");
        public string AdminSyncUrl => GetAdminUrl("sync/");

        public string GetAdminIndexUrl(int? siteId, string pageUrl)
        {
            var queryString = new NameValueCollection();
            if (siteId.HasValue && siteId.Value > 0)
            {
                queryString.Add("siteId", siteId.ToString());
            }
            if (!string.IsNullOrEmpty(pageUrl))
            {
                queryString.Add("pageUrl", PageUtils.UrlEncode(pageUrl));
            }
            return PageUtils.AddQueryString(AdminIndexUrl, queryString);
        }

        public string GetAdminContentsUrl(int siteId, int channelId)
        {
            return GetCmsUrl("contents", siteId, new
            {
                channelId
            });
        }

        public string GetAdminCreateStatusUrl(int siteId)
        {
            return GetCmsUrl("createStatus", siteId);
        }
    }
}
