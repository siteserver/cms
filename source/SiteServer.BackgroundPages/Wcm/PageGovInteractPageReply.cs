using System;
using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Wcm
{
	public class PageGovInteractPageReply : BasePageGovInteractPage
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, "待办理办件", AppManager.Wcm.Permission.WebSite.GovInteract);
            }
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, int contentId, string listPageUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractPageReply), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(listPageUrl)}
            });
        }
	}
}
