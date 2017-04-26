using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Analysis
{
	public class PageLogSite : BasePageCms
    {
        public DropDownList PublishmentSystem;
        public DropDownList LogType;
        public TextBox UserName;
        public TextBox Keyword;
        public DateTimeTextBox DateFrom;
        public DateTimeTextBox DateTo;

        public Repeater rptContents;
        public SqlPager spContents;
        public Literal ltlPublishmentSystem;

		public Button Delete;
		public Button DeleteAll;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = StringUtils.Constants.PageSize;

            if (!Body.IsQueryExists("LogType"))
            {
                spContents.SelectCommand = DataProvider.LogDao.GetSelectCommend();
            }
            else
            {
                spContents.SelectCommand = DataProvider.LogDao.GetSelectCommend(PublishmentSystemId, Body.GetQueryString("LogType"), Body.GetQueryString("UserName"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));
            }

            spContents.SortField = "ID";
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

            if (IsPostBack) return;

            BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Log, "站点日志", AppManager.Analysis.Permission.AnalysisLog);

            if (PublishmentSystemId == 0)
            {
                ltlPublishmentSystem.Text = @"<td align=""center"" width=""160"">&nbsp;站点名称</td>";
            }

            PublishmentSystem.Items.Add(new ListItem("<<全部站点>>", "0"));

            var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            foreach (var psId in publishmentSystemIdList)
            {
                PublishmentSystem.Items.Add(new ListItem(PublishmentSystemManager.GetPublishmentSystemInfo(psId).PublishmentSystemName, psId.ToString())); 
            }

            LogType.Items.Add(new ListItem("全部记录", "All"));
            LogType.Items.Add(new ListItem("栏目相关记录", "Channel"));
            LogType.Items.Add(new ListItem("内容相关记录", "Content"));

            if (Body.IsQueryExists("LogType"))
            {
                ControlUtils.SelectListItems(PublishmentSystem, PublishmentSystemId.ToString());
                ControlUtils.SelectListItems(LogType, Body.GetQueryString("LogType"));
                UserName.Text = Body.GetQueryString("UserName");
                Keyword.Text = Body.GetQueryString("Keyword");
                DateFrom.Text = Body.GetQueryString("DateFrom");
                DateTo.Text = Body.GetQueryString("DateTo");
            }

            if (Body.IsQueryExists("Delete"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                try
                {
                    DataProvider.LogDao.Delete(arraylist);
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("DeleteAll"))
            {
                try
                {
                    DataProvider.LogDao.DeleteAll();
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            Delete.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetAnalysisUrl(nameof(PageLogSite), new NameValueCollection
                    {
                        {"Delete", "True"}
                    }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));
            DeleteAll.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithConfirm(
                    PageUtils.GetAnalysisUrl(nameof(PageLogSite), new NameValueCollection
                    {
                        {"DeleteAll", "True"}
                    }), "此操作将删除所有日志信息，确定吗？"));

            spContents.DataBind();
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlPublishmentSystem = (Literal)e.Item.FindControl("ltlPublishmentSystem");
                var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
                var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
                var ltlIPAddress = (Literal)e.Item.FindControl("ltlIPAddress");
                var ltlAction = (Literal)e.Item.FindControl("ltlAction");
                var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

                if (PublishmentSystemId == 0)
                {
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(SqlUtils.EvalInt(e.Item.DataItem, "PublishmentSystemID"));
                    var publishmentSystemName = string.Empty;
                    if (publishmentSystemInfo != null)
                    {
                        publishmentSystemName =
                            $"<a href='{publishmentSystemInfo.PublishmentSystemUrl}' target='_blank'>{publishmentSystemInfo.PublishmentSystemName}</a>";
                    }
                    ltlPublishmentSystem.Text = $@"<td align=""center"" width=""160"">{publishmentSystemName}</td>";
                }
                ltlUserName.Text = SqlUtils.EvalString(e.Item.DataItem, "UserName");
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"));
                ltlIPAddress.Text = SqlUtils.EvalString(e.Item.DataItem, "IPAddress");
                ltlAction.Text = SqlUtils.EvalString(e.Item.DataItem, "Action");
                ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, "Summary");
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

	    private string PageUrl => PageUtils.GetAnalysisUrl(nameof(PageLogSite), new NameValueCollection
	    {
	        {"UserName", UserName.Text},
	        {"Keyword", Keyword.Text},
	        {"DateFrom", DateFrom.Text},
	        {"DateTo", DateTo.Text},
	        {"PublishmentSystemID", PublishmentSystem.SelectedValue},
	        {"LogType", LogType.SelectedValue}
	    });
	}
}
