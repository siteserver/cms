using System;
using SiteServer.BackgroundPages.Core;
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

            AuthResponse.AdminLogout();
            FxUtils.Page.Redirect(FxUtils.Page.GetLoginUrl());
        }
    }
}
