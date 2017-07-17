using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageConfiguration : BasePageCms
    {
        public Literal LtlIsDomain;
        public PlaceHolder PhDomain;
        public TextBox TbPublishmentSystemUrl;

        public Literal LtlPublishmentSystemUrl;
        public Literal LtlRedirectJs;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.IdConfiguration, "站点信息", AppManager.Permissions.WebSite.Configration);

                PhDomain.Visible = true;
                TbPublishmentSystemUrl.Text = PublishmentSystemInfo.PublishmentSystemUrl;

                var publishmentSystemUrl = PageUtils.AddProtocolToUrl(PageUtility.GetPublishmentSystemUrl(PublishmentSystemInfo, string.Empty));

                LtlPublishmentSystemUrl.Text =
                    $@"<a href=""{publishmentSystemUrl}"" id=""url"" target=""_blank"">{publishmentSystemUrl}</a>";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (PhDomain.Visible)
                {
                    PublishmentSystemInfo.PublishmentSystemUrl = TbPublishmentSystemUrl.Text;
                }

                try
                {
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
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
