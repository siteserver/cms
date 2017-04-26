using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundRight : BackgroundBasePageWX
    {
        public Literal ltlWelcome;
        public Literal ltlBinding;
        public Literal ltlDelete;

        public Literal ltlURL;
        public Literal ltlToken;

        public void Page_Load(object sender, System.EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Accounts, AppManager.WeiXin.LeftMenu.Function.ID_Info, string.Empty, AppManager.WeiXin.Permission.WebSite.Info);
                ltlWelcome.Text = "欢迎使用 SiteServer WeiXin 微信平台";

                var bindingUrl = ConsoleAccountBinding.GetRedirectUrl(PublishmentSystemID, PageUtils.GetWXUrl(
                    $"background_right.aspx?publishmentSystemID={PublishmentSystemID}"));

                var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);

                var isBinding = WeiXinManager.IsBinding(accountInfo);
                if (isBinding)
                {
                    ltlBinding.Text = $@"<a href=""{bindingUrl}"" class=""btn btn-success"">已绑定微信</a>";
                }
                else
                {
                    ltlBinding.Text = $@"<a href=""{bindingUrl}"" class=""btn btn-danger"">未绑定微信</a>";
                }

                ltlURL.Text = PageUtilityWX.API.GetMPUrl(PublishmentSystemID);

                ltlToken.Text = accountInfo.Token;

                var deleteUrl = PageUtils.GetSTLUrl(
                    $"console_publishmentSystemDelete.aspx?nodeID={PublishmentSystemID}&isBackgroundDelete=true");
                ltlDelete.Text = $@"<a href=""{deleteUrl}"" class=""btn btn-danger"">删除当前站点</a>";
            }
        }
    }
}
