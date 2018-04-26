using System.Collections.Specialized;
using SiteServer.CMS.Api;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageCreateStatus : BasePageCms
    {
        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageCreateStatus), new NameValueCollection
            {
                {"siteId", siteId.ToString()}
            });
        }

        public static string GetOpenLayerString(int siteId)
        {
            return LayerUtils.GetOpenScript("页面生成进度",
                PageUtils.GetSettingsUrl(nameof(PageCreateStatus), new NameValueCollection
                {
                    {"siteId", siteId.ToString()}
                }));
        }

        public static void Redirect(int siteId)
        {
            var pageUrl = PageUtils.GetSettingsUrl(nameof(PageCreateStatus), new NameValueCollection
            {
                {"siteId", siteId.ToString()}
            });
            PageUtils.Redirect(pageUrl);
        }

        public string RedirectUrl => PageRedirect.GetRedirectUrl(SiteId);

        public string SignalrHubsUrl = ApiManager.SignalrHubsUrl;

        //public void Page_Load(object sender, EventArgs e)
        //{
        //    if (IsForbidden) return;

        //    if (!IsPostBack)
        //    {
        //        base.BreadCrumb(AppManager.LeftMenu.ID_Utility, "生成队列", AppManager.Permission.Platform_Utility);
        //    }

        //    if (!string.IsNullOrEmpty(base.AuthRequest.GetQueryString("Cancel")))
        //    {
        //        DataProvider.CreateTaskDAO.DeleteAll(base.SiteId);
        //    }
        //}
    }
}
