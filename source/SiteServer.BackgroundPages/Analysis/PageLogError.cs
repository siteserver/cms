using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Analysis
{
	public class PageLogError : BasePage
    {
        public TextBox Keyword;
        public DateTimeTextBox DateFrom;
        public DateTimeTextBox DateTo;

        public Repeater rptContents;
        public SqlPager spContents;

		public Button Delete;
		public Button DeleteAll;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete"))
            {
                var list = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                try
                {
                    BaiRongDataProvider.ErrorLogDao.Delete(list);
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
                    BaiRongDataProvider.ErrorLogDao.DeleteAll();
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = StringUtils.Constants.PageSize;

            if (!Body.IsQueryExists("Keyword"))
            {
                spContents.SelectCommand = BaiRongDataProvider.ErrorLogDao.GetSelectCommend();
            }
            else
            {
                spContents.SelectCommand = BaiRongDataProvider.ErrorLogDao.GetSelectCommend(Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));
            }

            spContents.SortField = "ID";
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
			{
                BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Log, "系统错误日志", AppManager.Analysis.Permission.AnalysisLog);

                if (Body.IsQueryExists("Keyword"))
                {
                    Keyword.Text = Body.GetQueryString("Keyword");
                    DateFrom.Text = Body.GetQueryString("DateFrom");
                    DateTo.Text = Body.GetQueryString("DateTo");
                }

                Delete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetAnalysisUrl(nameof(PageLogError), new NameValueCollection
                {
                    {"Delete", "True" }
                }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

                DeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.GetAnalysisUrl(nameof(PageLogError), new NameValueCollection
                {
                    {"DeleteAll", "True" }
                }), "此操作将删除所有日志信息，确定吗？"));

                spContents.DataBind();
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
                var ltlMessage = (Literal)e.Item.FindControl("ltlMessage");
                var ltlStacktrace = (Literal)e.Item.FindControl("ltlStacktrace");
                var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

                ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"));
                ltlMessage.Text = SqlUtils.EvalString(e.Item.DataItem, "Message");
                ltlStacktrace.Text = SqlUtils.EvalString(e.Item.DataItem, "Stacktrace");
                ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, "Summary");
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

	    private string PageUrl => PageUtils.GetAnalysisUrl(nameof(PageLogError), new NameValueCollection
	    {
	        {"Keyword", Keyword.Text},
	        {"DateFrom", DateFrom.Text},
	        {"DateTo", DateTo.Text}
	    });
	}
}
