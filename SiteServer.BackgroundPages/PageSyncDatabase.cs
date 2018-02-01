using System;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages
{
    public class PageSyncDatabase : BasePage
    {
        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSiteServerUrl(nameof(PageSyncDatabase), null);
        }

        public string UpdateDatabaseApiUrl => ApiRouteSyncDatabase.GetUrl(PageUtility.InnerApiUrl);

        public string AdminUrl => PageUtils.GetAdminDirectoryUrl(string.Empty);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (SystemManager.IsNeedInstall())
            {
                Page.Response.Write("系统未安装，向导被禁用");
                Page.Response.End();
            }
        }
    }
}
