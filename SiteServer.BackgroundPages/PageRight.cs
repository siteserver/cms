using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Sys.Administrators;

namespace SiteServer.BackgroundPages
{
    public class PageRight : BasePage
    {
        public Literal LtlVersionInfo;
        public Literal LtlUpdateDate;
        public Literal LtlLastLoginDate;

        public string ApiUrl => SiteCheckList.GetUrl(PageUtils.InnerApiUrl, Body.AdminName);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            LtlVersionInfo.Text = AppManager.GetFullVersion();

            if (Body.AdministratorInfo.LastActivityDate != DateTime.MinValue)
            {
                LtlLastLoginDate.Text = DateUtils.GetDateAndTimeString(Body.AdministratorInfo.LastActivityDate);
            }

            LtlUpdateDate.Text = DateUtils.GetDateAndTimeString(ConfigManager.Instance.UpdateDate);
        }
	}
}
