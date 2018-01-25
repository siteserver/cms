using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Controllers.Sys.Administrators;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages
{
    public class PageRight : BasePage
    {
        public Literal LtlVersionInfo;
        public Literal LtlUpdateDate;
        public Literal LtlLastLoginDate;

        public string ApiUrl => SiteCheckList.GetUrl(PageUtility.InnerApiUrl, Body.AdminName);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            LtlVersionInfo.Text = SystemManager.Version;

            if (Body.AdministratorInfo.LastActivityDate != DateTime.MinValue)
            {
                LtlLastLoginDate.Text = DateUtils.GetDateAndTimeString(Body.AdministratorInfo.LastActivityDate);
            }

            LtlUpdateDate.Text = DateUtils.GetDateAndTimeString(ConfigManager.Instance.UpdateDate);
        }
	}
}
