using System;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages
{
    public class PageLogout : BasePage
    {
        protected override bool IsAccessable => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var redirectUrl = PageUtils.GetAdminDirectoryUrl("login.aspx");

            Body.AdminLogout();
            PageUtils.Redirect(PageUtils.ParseNavigationUrl(redirectUrl));
        }
    }
}
