using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelsGroup : BasePageCms
    {
        public Literal LtlChannelGroupName;
        public Repeater RptContents;
        private string _nodeGroupName;

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

            _nodeGroupName = Body.GetQueryString("nodeGroupName");

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            LtlChannelGroupName.Text = "栏目组：" + _nodeGroupName;

            RptContents.DataSource = DataProvider.NodeDao.GetNodeIdListByGroupName(PublishmentSystemId, _nodeGroupName);
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

            if (IsOwningNodeId(nodeId))
            {
                if (AdminUtility.HasChannelPermissions(Body.AdminName, nodeInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelEdit))
                {
                    ltlItemChannelName.Text = $@"<a href=""javascript:;"" onclick=""{ModalChannelEdit.GetOpenWindowString(nodeInfo.PublishmentSystemId, nodeInfo.NodeId, GetRedirectUrl(nodeInfo.PublishmentSystemId, _nodeGroupName))}"">{ltlItemChannelName.Text}</a>";
                }
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageNodeGroup.GetRedirectUrl(PublishmentSystemId));
        }
    }
}
