namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageNavFunction : BasePageCms
    {
		public void Page_Load(object sender, System.EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.Id_Site, "微功能大全", AppManager.Permission.Platform_Site);
            }
		}
	}
}
