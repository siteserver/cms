using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Sys;
using SiteServer.CMS.Core.Security;

namespace SiteServer.BackgroundPages
{
	public class PageInitialization : BasePageCms
    {
        public Literal LtlContent;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSiteServerUrl(nameof(PageInitialization), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (PageUtils.DetermineRedirectToInstaller()) return;

            if (!Body.IsAdministratorLoggin)
            {
                PageUtils.RedirectToLoginPage();
                return;
            }

            if (Body.AdministratorInfo.IsLockedOut)
            {
                PageUtils.RedirectToLoginPage("对不起，您的账号已被锁定，无法进入系统！");
                return;
            }

            var redirectUrl = PageMain.GetRedirectUrl();

            var permissions = PermissionsManager.GetPermissions(Body.AdministratorName);
            var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;
            if (publishmentSystemIdList == null || publishmentSystemIdList.Count == 0)
            {
                if (permissions.IsSystemAdministrator)
                {
                    redirectUrl = PageAppAdd.GetRedirectUrl();
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
";
        }
	}
}
