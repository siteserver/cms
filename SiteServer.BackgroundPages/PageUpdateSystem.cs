using System;
using System.Web.UI.WebControls;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;

namespace SiteServer.BackgroundPages
{
    public class PageUpdateSystem : BasePage
    {
        public Button BtnUpload;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSiteServerUrl(nameof(PageUpdateSystem), null);
        }

        public string PackageId => PackageUtils.PackageIdSsCms;

        public string InstalledVersion => SystemManager.Version;

        public string AdminUrl => PageUtils.GetAdminDirectoryUrl(string.Empty);

        public string DownloadApiUrl => ApiRouteDownload.GetUrl(PageUtility.InnerApiUrl);

        public string UpdateApiUrl => ApiRouteUpdate.GetUrl(PageUtility.InnerApiUrl);

        public string UpdateSsCmsApiUrl => ApiRouteUpdateSsCms.GetUrl(PageUtility.InnerApiUrl);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (SystemManager.IsNeedInstall())
            {
                Page.Response.Write("系统未安装，向导被禁用");
                Page.Response.End();
                return;
            }

            BtnUpload.OnClientClick = ModalManualUpdateSystem.GetOpenWindowString();
        }
    }
}
