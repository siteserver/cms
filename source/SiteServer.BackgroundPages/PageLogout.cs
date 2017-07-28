using System;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages
{
    public class PageLogout : BasePage
    {
        protected override bool IsAccessable => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var redirectUrl = PageUtils.GetAdminDirectoryUrl("login.aspx");

            RequestBody.AdministratorLogout();
            PageUtils.Redirect(PageUtils.ParseNavigationUrl(redirectUrl));
        }
    }
}
