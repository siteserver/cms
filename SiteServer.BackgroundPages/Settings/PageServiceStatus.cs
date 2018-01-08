using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageServiceStatus : BasePage
    {
        public PlaceHolder PhOffline;
        public PlaceHolder PhOnline;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Service);

            var isOnline = ServiceManager.IsServiceOnline;
            PhOffline.Visible = !isOnline;
            PhOnline.Visible = isOnline;
        }
	}
}
