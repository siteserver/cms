using SiteServer.CMS.BackgroundPages;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class BackgroundNavFunction : BackgroundBasePage
	{
		public void Page_Load(object sender, System.EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.ID_Site, "微功能大全", AppManager.Permission.Platform_Site);
            }
		}
	}
}
