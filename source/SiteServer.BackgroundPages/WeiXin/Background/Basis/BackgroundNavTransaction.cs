using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundNavTransaction : BackgroundBasePage
    {
        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_navTransaction.aspx?PublishmentSystemID={publishmentSystemID}");
        }
        public void Page_Load(object sender, System.EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.ID_Site, "会员交易", AppManager.Permission.Platform_Site);
            }
        }
    }
}
