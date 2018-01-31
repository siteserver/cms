using System;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using ApiRouteVersion = SiteServer.CMS.Controllers.Sys.Packaging.ApiRouteVersion;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageUpdate : BasePage
    {
        public static string GetRedirectUrl()
        {
            return PageUtils.GetPluginsUrl(nameof(PageUpdate), null);
        }

        public string AdminUrl => PageUtils.GetAdminDirectoryUrl(string.Empty);

        public string VersionApiUrl => ApiRouteVersion.GetPluginsUrl(PageUtility.InnerApiUrl);

        public string DownloadApiUrl => ApiRouteDownload.GetUrl(PageUtility.InnerApiUrl);

        public string UpdateApiUrl => ApiRouteUpdate.GetUrl(PageUtility.InnerApiUrl);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Plugins.Add, ConfigManager.Permissions.Plugins.Management);
        }
    }
}
