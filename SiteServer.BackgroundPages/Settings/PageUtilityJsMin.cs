using System;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageUtilityJsMin : BasePage
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                VerifyAdministratorPermissions(AppManager.Permissions.Settings.Utility);
            }
        }
	}
}
