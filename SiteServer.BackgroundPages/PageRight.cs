using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Administrators;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;

namespace SiteServer.BackgroundPages
{
    public class PageRight : BasePage
    {
        public Literal LtlVersionInfo;
        public Literal LtlUpdateDate;
        public Literal LtlLastLoginDate;

        public string SiteCheckListApiUrl => ApiRouteSiteCheckList.GetUrl(ApiManager.InnerApiUrl, AuthRequest.AdminName);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            LtlVersionInfo.Text = SystemManager.Version == PackageUtils.VersionDev ? "dev" : SystemManager.Version;

            if (AuthRequest.AdminInfo.LastActivityDate != DateTime.MinValue)
            {
                LtlLastLoginDate.Text = DateUtils.GetDateAndTimeString(AuthRequest.AdminInfo.LastActivityDate);
            }

            LtlUpdateDate.Text = DateUtils.GetDateAndTimeString(ConfigManager.Instance.UpdateDate);
        }
	}
}
