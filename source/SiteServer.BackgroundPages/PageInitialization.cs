using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core.Permissions;
using SiteServer.CMS.Core.Security;

namespace SiteServer.BackgroundPages
{
	public class PageInitialization : BasePageCms
    {
        public Literal LtlContent;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl() // 本页面实际地址获取函数  如果需要从其他地方跳转到本页面，则调用此方法即可
        {
            return PageUtils.GetSiteServerUrl(nameof(PageInitialization), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return; // 检测是否允许访问本页面

            if (PageUtils.DetermineRedirectToInstaller()) return; // 检测系统是否需要安装，如果需要转到安装页面。

            if (!Body.IsAdministratorLoggin) // 检测管理员是否登录
            {
                PageUtils.RedirectToLoginPage(); // 如果没有登录则跳到登录页面
                return;
            }

            if (Body.AdministratorInfo.IsLockedOut) // 检测管理员帐号是否被锁定
            {
                PageUtils.RedirectToLoginPage("对不起，您的账号已被锁定，无法进入系统！");
                return;
            }

            var redirectUrl = PageMain.GetRedirectUrl(); // 如果检测登录帐号一切正常，则准备转到框架主页 pagemain.aspx

            var permissions = PermissionsManager.GetPermissions(Body.AdministratorName); // 获取登录管理员的权限
            var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList; // 获取当前站点ID
            if (publishmentSystemIdList == null || publishmentSystemIdList.Count == 0) // 如果目前还没有创建站点
            {
                if (permissions.IsSystemAdministrator)  // 如果目前还没有创建站点并且当前登录管理员是系统管理员
                {
                    redirectUrl = PagePublishmentSystemAdd.GetRedirectUrl(); // 则直接跳到站点创建页面
                }
            }

            LtlContent.Text = $@"
<script language=""javascript"">
function redirectUrl()
{{
   location.href = ""{redirectUrl}"";
}}
setTimeout(""redirectUrl()"", 2000);
</script>
";   // 通过输出js来实现2秒之后开始页面跳转
        }
	}
}
