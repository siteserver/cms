using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerChannelAnalysis : BasePageCms
    {
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
		public Repeater rptContents;

        private Hashtable _accessNumHashtableToChannel = new Hashtable();
        private Hashtable _accessNumHashtableToContent = new Hashtable();
        private int _totalAccessNum;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTrackerChannelAnalysis), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "栏目流量统计", AppManager.Cms.Permission.WebSite.Tracking);

                StartDate.Text = string.Empty;
                EndDate.Now = true;

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", NavigationTreeItem.GetNodeTreeScript());

                BindGrid();
			}
		}

        public void BindGrid()
        {
            try
            {
                var begin = DateUtils.SqlMinValue;
                if (!string.IsNullOrEmpty(StartDate.Text))
                {
                    begin = TranslateUtils.ToDateTime(StartDate.Text);
                }
                _accessNumHashtableToChannel = DataProvider.TrackingDao.GetChannelAccessNumHashtable(PublishmentSystemId, begin, TranslateUtils.ToDateTime(EndDate.Text));
                _accessNumHashtableToContent = DataProvider.TrackingDao.GetChannelContentAccessNumHashtable(PublishmentSystemId, begin, TranslateUtils.ToDateTime(EndDate.Text));

                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
                foreach (var nodeId in nodeIdList)
                {
                    var accessNum = 0;
                    if (_accessNumHashtableToChannel[nodeId] != null)
                    {
                        accessNum = Convert.ToInt32(_accessNumHashtableToChannel[nodeId]);
                    }
                    if (_accessNumHashtableToContent[nodeId] != null)
                    {
                        accessNum += Convert.ToInt32(_accessNumHashtableToContent[nodeId]);
                    }
                    _totalAccessNum += accessNum;
                }

                rptContents.DataSource = nodeIdList;
                rptContents.ItemDataBound += rptContents_ItemDataBound;
                rptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeID = (int)e.Item.DataItem;
            var enabled = (IsOwningNodeId(nodeID)) ? true : false;
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeID)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);

            var ltlTrHtml = (Literal)e.Item.FindControl("ltlTrHtml");
            var ltlNodeTitle = (Literal)e.Item.FindControl("ltlNodeTitle");
            var ltlAccessNumBar = (Literal)e.Item.FindControl("ltlAccessNumBar");
            var ltlItemView = (Literal)e.Item.FindControl("ltlItemView");
            var ltlChannelCount = (Literal)e.Item.FindControl("ltlChannelCount");
            var ltlContentCount = (Literal)e.Item.FindControl("ltlContentCount");
            var ltlTotalCount = (Literal)e.Item.FindControl("ltlTotalCount");

            ltlTrHtml.Text =
                $@"<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"" style=""height:20px;line-height:20px;{StringUtils.Constants
                    .ShowElementStyle}"">";
            ltlNodeTitle.Text = GetTitle(nodeInfo, enabled);

            var accessNumToChannel = 0;
            if (_accessNumHashtableToChannel[nodeID] != null)
            {
                accessNumToChannel = Convert.ToInt32(_accessNumHashtableToChannel[nodeID]);
            }
            var accessNumToContent = 0;
            if (_accessNumHashtableToContent[nodeID] != null)
            {
                accessNumToContent = Convert.ToInt32(_accessNumHashtableToContent[nodeID]);
            }

            var accessNum = accessNumToChannel + accessNumToContent;

            ltlAccessNumBar.Text = $@"<div class=""progress progress-success progress-striped"">
            <div class=""bar"" style=""width: {GetAccessNumBarWidth(accessNum)}%""></div>
          </div>";

            //ltlItemView.Text = $@"<a href=""javascript:;"" onclick=""{ModalTrackerIPView.GetOpenWindowString(StartDate.Text, EndDate.Text, PublishmentSystemID, nodeID, 0, accessNum)}"">详细</a>";

            ltlChannelCount.Text = accessNumToChannel.ToString();
            ltlContentCount.Text = accessNumToContent.ToString();
            ltlTotalCount.Text = accessNum.ToString();
        }

        private double GetAccessNumBarWidth(int accessNum)
        {
            double width = 0;
            if (_totalAccessNum > 0)
            {
                width = Convert.ToDouble(accessNum) / Convert.ToDouble(_totalAccessNum);
                width = Math.Round(width, 2) * 200;
            }
            return width;
        }

		public void Analysis_OnClick(object sender, EventArgs e)
		{
			BindGrid();
		}

	    private string GetTitle(NodeInfo nodeInfo, bool enabled)
        {
            var showPopWinString = ModalChannelEdit.GetOpenWindowString(PublishmentSystemId, nodeInfo.NodeId, GetRedirectUrl(PublishmentSystemId));

            var hasChildren = nodeInfo.ChildrenCount > 0;

            var nodeTreeItem = NodeNaviTreeItem.CreateNodeTreeItem(true, true, nodeInfo.ParentsCount, hasChildren, nodeInfo.NodeName, string.Empty, showPopWinString, string.Empty, enabled, false, PublishmentSystemId, nodeInfo.NodeId, nodeInfo.ContentNum);
            return nodeTreeItem.GetItemHtml();
        }
	}
}
