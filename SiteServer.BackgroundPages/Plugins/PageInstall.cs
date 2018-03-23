using System;
using System.Collections.Specialized;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageInstall : BasePage
    {
        public static string GetRedirectUrl(bool isUpdate, string packageId)
        {
            return PageUtils.GetPluginsUrl(nameof(PageInstall), new NameValueCollection
            {
                { "type", isUpdate ? "update" : "install" },
                { "packageId", packageId }
            });
        }

        public string Type => AuthRequest.GetQueryString("type") == "update" ? "升级" : "安装";

        public string AdminUrl => PageUtils.GetAdminDirectoryUrl(string.Empty);

        public string PackagesIdAndVersionList => TranslateUtils.JsonSerialize(PluginManager.PackagesIdAndVersionList);

        public string PackageId { get; set; }

        public string DownloadApiUrl => ApiRouteDownload.GetUrl(ApiManager.InnerApiUrl);

        public string UpdateApiUrl => ApiRouteUpdate.GetUrl(ApiManager.InnerApiUrl);

        public string ClearCacheApiUrl => ApiRouteClearCache.GetUrl(ApiManager.InnerApiUrl);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PackageId = AuthRequest.GetQueryString("packageId");

            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.PluginsPermissions.Add, ConfigManager.PluginsPermissions.Management);
        }
    }
}
