using System;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovInteractListCheck : BasePageGovInteractList
	{
        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractListCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, "待审核办件", AppManager.Wcm.Permission.WebSite.GovInteract);
            }
        }

        protected override string GetSelectString()
        {
            return DataProvider.GovInteractContentDao.GetSelectStringByState(PublishmentSystemInfo, nodeID, EGovInteractState.Replied);
        }

        private string _pageUrl;
        protected override string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = GetRedirectUrl(PublishmentSystemId, nodeID);
                }
                return _pageUrl;
            }
        }
	}
}
