using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerContentAnalysis : BasePageCms
    {
        private int nodeID;
        private string startDateString;
        private string endDateString;

        private Hashtable accessNumHashtable = new Hashtable();

        public DropDownList NodeIDDropDownList;
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
        public Repeater rptContents;
        public SqlPager spContents;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("NodeID"))
            {
                nodeID = int.Parse(Body.GetQueryString("NodeID"));
                startDateString = Body.GetQueryString("StartDateString");
                endDateString = Body.GetQueryString("EndDateString");
            }
            else
            {
                nodeID = PublishmentSystemId;
                startDateString = string.Empty;
                endDateString = DateTime.Now.ToString(DateUtils.FormatStringDateOnly);
            }

            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeID, EScopeType.Self, string.Empty, string.Empty);
            spContents.SelectCommand = BaiRongDataProvider.ContentDao.GetSelectCommend(tableName, nodeIdList, ETriState.True);
            spContents.SortField = BaiRongDataProvider.ContentDao.GetSortFieldName();
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
            {
                BreadCrumbWithItemTitle(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "内容流量统计", NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeID), AppManager.Cms.Permission.WebSite.Tracking);

                StartDate.Text = startDateString;
                EndDate.Text = endDateString;

                NodeManager.AddListItems(NodeIDDropDownList.Items, PublishmentSystemInfo, true, true, Body.AdministratorName);
                ControlUtils.SelectListItems(NodeIDDropDownList, nodeID.ToString());

                var begin = DateUtils.SqlMinValue;
                if (!string.IsNullOrEmpty(startDateString))
                {
                    begin = TranslateUtils.ToDateTime(startDateString);
                }
                accessNumHashtable = DataProvider.TrackingDao.GetContentAccessNumHashtable(PublishmentSystemId, nodeID, begin, TranslateUtils.ToDateTime(endDateString));

                spContents.DataBind();
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var itemTitle = (Literal)e.Item.FindControl("ItemTitle");
                var itemView = (Literal)e.Item.FindControl("ItemView");
                var itemCount = (Literal)e.Item.FindControl("ItemCount") ;

                var contentInfo = new BackgroundContentInfo(e.Item.DataItem);

                itemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, string.Empty);

                var num = 0;
                if (accessNumHashtable[contentInfo.Id] != null)
                {
                    num = Convert.ToInt32(accessNumHashtable[contentInfo.Id]);
                }
                itemCount.Text = num.ToString();

                //itemView.Text = $@"<a href=""javascript:;"" onclick=""{TrackerIPView.GetOpenWindowString(startDateString, endDateString, PublishmentSystemID, nodeID, contentInfo.Id, num)}"">详细</a>";
            }
        }

		public void Analysis_OnClick(object sender, EventArgs e)
		{
            PageUtils.Redirect(PageUtils.GetCmsUrl(nameof(PageTrackerContentAnalysis), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString() },
                {"NodeID", nodeID.ToString() },
                {"StartDateString", StartDate.Text },
                {"EndDateString", EndDate.Text }
            }));
		}

        public void NodeIDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUtils.GetCmsUrl(nameof(PageTrackerContentAnalysis), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString() },
                {"NodeID", NodeIDDropDownList.SelectedValue },
                {"StartDateString", startDateString },
                {"EndDateString", endDateString }
            }));
        }
	}
}
