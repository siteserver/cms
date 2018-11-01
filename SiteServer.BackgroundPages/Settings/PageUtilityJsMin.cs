using System;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageUtilityJsMin : BasePage
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                VerifySystemPermissions(ConfigManager.SettingsPermissions.Utility);
            }
        }
	}
}
