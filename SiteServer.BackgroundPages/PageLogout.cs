using System;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages
{
    public class PageLogout : BasePage
    {
        protected override bool IsAccessable => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            AuthRequest.AdminLogout();
            PageUtilsEx.Redirect(PageUtilsEx.GetLoginUrl());
        }
    }
}
