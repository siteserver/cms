using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;

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

            var redirectUrl = PageMain.GetRedirectUrl(); // 如果检测登录帐号一切正常，则准备转到框架主页 pagemain.aspx

            var siteIdList = AuthRequest.AdminPermissionsImpl.GetSiteIdList(); // 获取当前站点ID
            if (siteIdList == null || siteIdList.Count == 0) // 如果目前还没有创建站点
            {
                if (AuthRequest.AdminPermissionsImpl.IsSystemAdministrator)  // 如果目前还没有创建站点并且当前登录管理员是系统管理员
                {
                    redirectUrl = PageSiteAdd.GetRedirectUrl(); // 则直接跳到站点创建页面
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
