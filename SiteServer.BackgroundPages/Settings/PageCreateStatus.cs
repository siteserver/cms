using System;
using System.Collections.Specialized;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageCreateStatus : BasePageCms
    {
        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageCreateStatus), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public static string GetOpenLayerString(int publishmentSystemId)
        {
            return LayerUtils.GetOpenScript("页面生成进度",
                PageUtils.GetSettingsUrl(nameof(PageCreateStatus), new NameValueCollection
                {
                    {"publishmentSystemID", publishmentSystemId.ToString()}
                }));
        }

        public static void Redirect(int publishmentSystemId)
        {
            var pageUrl = PageUtils.GetSettingsUrl(nameof(PageCreateStatus), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()}
            });
            PageUtils.Redirect(pageUrl);
        }

        public string SiteUrl => PageRedirect.GetRedirectUrl(PublishmentSystemId);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                //base.BreadCrumb(AppManager.LeftMenu.ID_Utility, "生成队列", AppManager.Permission.Platform_Utility);
            }

            //if (!string.IsNullOrEmpty(base.Body.GetQueryString("Cancel")))
            //{
            //    DataProvider.CreateTaskDAO.DeleteAll(base.PublishmentSystemID);
            //}
        }
    }
}
