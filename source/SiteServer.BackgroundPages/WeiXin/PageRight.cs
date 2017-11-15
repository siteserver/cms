using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Manager;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageRight : BasePageCms
    {
        public Literal LtlWelcome;
        public Literal LtlBinding;
        public Literal LtlDelete;

        public Literal LtlUrl;
        public Literal LtlToken;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageRight), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, System.EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.IdAccounts, string.Empty, AppManager.WeiXin.Permission.WebSite.Info);
                LtlWelcome.Text = "欢迎使用 SiteServer CMS 微信站点";

                var bindingUrl = PageAccountBinding.GetRedirectUrl(PublishmentSystemId, GetRedirectUrl(PublishmentSystemId));

                var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);

                var isBinding = WeiXinManager.IsBinding(accountInfo);
                if (isBinding)
                {
                    LtlBinding.Text = $@"<a href=""{bindingUrl}"" class=""btn btn-success"">已绑定微信</a>";
                }
                else
                {
                    LtlBinding.Text = $@"<a href=""{bindingUrl}"" class=""btn btn-danger"">未绑定微信</a>";
                }

                LtlUrl.Text = PageUtilityWX.API.GetMPUrl(PublishmentSystemId);

                LtlToken.Text = accountInfo.Token;

                var deleteUrl = PagePublishmentSystemDelete.GetRedirectUrl(PublishmentSystemId);
                LtlDelete.Text = $@"<a href=""{deleteUrl}"" class=""btn btn-danger"">删除当前站点</a>";
            }
        }
    }
}
