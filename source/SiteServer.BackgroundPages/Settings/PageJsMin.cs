using System;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageJsMin : BasePage
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Utility, "JS脚本压缩", AppManager.Settings.Permission.SettingsUtility);
            }
        }
	}
}
