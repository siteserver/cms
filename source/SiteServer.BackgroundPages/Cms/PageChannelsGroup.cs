using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelsGroup : BasePageCms
    {
        public Literal LtlChannelGroupName;

        public Repeater RptContents;

        public static string GetRedirectUrl(int publishmentSystemId, string nodeGroupName)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelsGroup), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"nodeGroupName", nodeGroupName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var nodeGroupName = Body.GetQueryString("nodeGroupName");

            if (IsPostBack) return;

            BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationGroupAndTags, "查看栏目组", AppManager.Cms.Permission.WebSite.Configration);

            LtlChannelGroupName.Text = "栏目组：" + nodeGroupName;

            RptContents.DataSource = DataProvider.NodeDao.GetNodeIdListByGroupName(PublishmentSystemId, nodeGroupName);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var nodeId = (int)e.Item.DataItem;
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            if (nodeInfo == null)
            {
                e.Item.Visible = false;
                return;
            }

            var ltlItemChannelName = (Literal)e.Item.FindControl("ltlItemChannelName");
            var ltlItemChannelIndex = (Literal)e.Item.FindControl("ltlItemChannelIndex");
            var ltlItemAddDate = (Literal)e.Item.FindControl("ltlItemAddDate");

            ltlItemChannelName.Text = NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeId);
            ltlItemChannelIndex.Text = nodeInfo.NodeIndexName;
            ltlItemAddDate.Text = DateUtils.GetDateString(nodeInfo.AddDate);
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageNodeGroup.GetRedirectUrl(PublishmentSystemId));
        }
    }
}
