using System;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageAdd : BasePage
    {
        public static string GetRedirectUrl()
        {
            return PageUtils.GetPluginsUrl(nameof(PageAdd), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Page.IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Plugins.Add);
        }
    }
}
