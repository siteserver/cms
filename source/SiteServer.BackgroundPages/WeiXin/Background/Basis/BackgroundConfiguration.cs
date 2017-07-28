using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.BackgroundPages;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundConfiguration : BackgroundBasePage
    {
        public Literal ltlIsDomain;
        public PlaceHolder phDomain;
        public TextBox tbPublishmentSystemUrl;

        public Literal ltlIsPoweredBy;
        public PlaceHolder phPoweredBy;
        public TextBox tbPoweredBy;

        public Literal ltlPublishmentSystemUrl;
        public Literal ltlRedirectJs;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.CMS.LeftMenu.ID_Configration, "站点信息", AppManager.CMS.Permission.WebSite.Configration);

                phDomain.Visible = true;
                tbPublishmentSystemUrl.Text = PublishmentSystemInfo.PublishmentSystemUrl;

                phPoweredBy.Visible = true;
                tbPoweredBy.Text = PublishmentSystemInfo.Additional.WX_PoweredBy;

                var publishmentSystemUrl = PageUtils.AddProtocolToUrl(PageUtility.GetPublishmentSystemUrl(PublishmentSystemInfo, string.Empty));

                ltlPublishmentSystemUrl.Text =
                    $@"<a href=""{publishmentSystemUrl}"" id=""url"" target=""_blank"">{publishmentSystemUrl}</a>";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (phDomain.Visible)
                {
                    PublishmentSystemInfo.PublishmentSystemUrl = tbPublishmentSystemUrl.Text;
                }
                if (phPoweredBy.Visible)
                {
                    PublishmentSystemInfo.Additional.WX_IsPoweredBy = true;
                    PublishmentSystemInfo.Additional.WX_PoweredBy = tbPoweredBy.Text;
                }

                try
                {
                    DataProvider.PublishmentSystemDAO.Update(PublishmentSystemInfo);
                    SuccessMessage("站点信息设置修改成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "站点信息设置修改失败！");
                }
            }
        }
    }
}
