using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageNavTransaction : BasePageCms
    {
        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageNavTransaction), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, System.EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.Id_Site, "会员交易", AppManager.Permission.Platform_Site);
            }
        }
    }
}
