using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelsGroup : BasePageCms
    {
        public Literal LtlChannelGroupName;
        public Repeater RptContents;
        private string _nodeGroupName;

        public static string GetRedirectUrl(int siteId, string nodeGroupName)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannelsGroup), new NameValueCollection
            {
                {"nodeGroupName", nodeGroupName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeGroupName = AuthRequest.GetQueryString("nodeGroupName");

            if (IsPostBack) return;

            VerifySitePermissions(Constants.WebSitePermissions.Configuration);

            LtlChannelGroupName.Text = "栏目组：" + _nodeGroupName;

            var channelInfo = ChannelManager.GetChannelAsync(SiteId, SiteId).GetAwaiter().GetResult();
            RptContents.DataSource = ChannelManager.GetChannelIdListAsync(channelInfo, EScopeType.All, _nodeGroupName, string.Empty, string.Empty).GetAwaiter().GetResult();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var channelId = (int)e.Item.DataItem;
            var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();

            if (nodeInfo == null)
            {
                e.Item.Visible = false;
                return;
            }

            var ltlItemChannelName = (Literal)e.Item.FindControl("ltlItemChannelName");
            var ltlItemChannelIndex = (Literal)e.Item.FindControl("ltlItemChannelIndex");
            var ltlItemAddDate = (Literal)e.Item.FindControl("ltlItemAddDate");

            ltlItemChannelName.Text = ChannelManager.GetChannelNameNavigationAsync(SiteId, channelId).GetAwaiter().GetResult();
            ltlItemChannelIndex.Text = nodeInfo.IndexName;
            ltlItemAddDate.Text = DateUtils.GetDateString(nodeInfo.AddDate);

            if (IsOwningChannelId(channelId))
            {
                if (HasChannelPermissions(nodeInfo.Id, Constants.ChannelPermissions.ChannelEdit))
                {
                    ltlItemChannelName.Text = $@"<a href=""javascript:;"" onclick=""{ModalChannelEdit.GetOpenWindowString(nodeInfo.SiteId, nodeInfo.Id, GetRedirectUrl(nodeInfo.SiteId, _nodeGroupName))}"">{ltlItemChannelName.Text}</a>";
                }
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageNodeGroup.GetRedirectUrl(SiteId));
        }
    }
}
