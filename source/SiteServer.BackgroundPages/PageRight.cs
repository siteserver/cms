using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Administrators;

namespace SiteServer.BackgroundPages
{
    public class PageRight : BasePage
    {
        public Literal LtlWelcome;
        public Literal LtlVersionInfo;
        public Literal LtlUpdateDate;
        public Literal LtlLastLoginDate;

        public string ApiUrl => SiteCheckList.GetUrl(PageUtils.GetApiUrl(), Body.AdministratorName);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            LtlWelcome.Text = "欢迎使用 SiteServer 管理后台";

            LtlVersionInfo.Text = AppManager.GetFullVersion();

            if (Body.AdministratorInfo.LastActivityDate != DateTime.MinValue)
            {
                LtlLastLoginDate.Text = DateUtils.GetDateAndTimeString(Body.AdministratorInfo.LastActivityDate);
            }

            LtlUpdateDate.Text = DateUtils.GetDateAndTimeString(ConfigManager.Instance.UpdateDate);
        }
	}
}
