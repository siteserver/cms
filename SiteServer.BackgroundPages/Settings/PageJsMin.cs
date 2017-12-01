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
                BreadCrumbSettings("JS脚本压缩", AppManager.Permissions.Settings.Utility);
            }
        }
	}
}
