using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageRecord : Page
    {
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;

		public Button BtnDelete;
		public Button BtnDeleteAll;

        public void Page_Load(object sender, EventArgs e)
        {
            if (!BaiRongDataProvider.RecordDao.IsRecord()) return;

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IdCollection"]);
                BaiRongDataProvider.RecordDao.Delete(list);
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["DeleteAll"]))
            {
                BaiRongDataProvider.RecordDao.DeleteAll();
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 100;

            SpContents.SelectCommand = string.IsNullOrEmpty(Request.QueryString["Keyword"]) ? BaiRongDataProvider.RecordDao.GetSelectCommend() : BaiRongDataProvider.RecordDao.GetSelectCommend(Request.QueryString["Keyword"], Request.QueryString["DateFrom"], Request.QueryString["DateTo"]);

            SpContents.SortField = "Id";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

			if(!IsPostBack)
			{
                if (!string.IsNullOrEmpty(Request.QueryString["Keyword"]))
                {
                    TbKeyword.Text = Request.QueryString["Keyword"];
                    TbDateFrom.Text = Request.QueryString["DateFrom"];
                    TbDateTo.Text = Request.QueryString["DateTo"];
                }

                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetSettingsUrl(nameof(PageRecord), new NameValueCollection
                {
                    {"Delete", "True" }
                }), "IDCollection", "IDCollection", "请选择需要删除的记录！", "此操作将删除所选记录，确认吗？"));

                BtnDeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.GetSettingsUrl(nameof(PageRecord), new NameValueCollection
                {
                    {"DeleteAll", "True" }
                }), "此操作将删除所有记录信息，确定吗？"));

                SpContents.DataBind();
			}
		}

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlText = (Literal)e.Item.FindControl("ltlText");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");
            var ltlSource = (Literal)e.Item.FindControl("ltlSource");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");

            ltlText.Text = SqlUtils.EvalString(e.Item.DataItem, "Text");
            ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, "Summary");
            ltlSource.Text = SqlUtils.EvalString(e.Item.DataItem, "Source");
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"), EDateFormatType.Day, ETimeFormatType.LongTime);
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

	    private string PageUrl => PageUtils.GetSettingsUrl(nameof(PageRecord), new NameValueCollection
	    {
	        {"Keyword", TbKeyword.Text},
	        {"DateFrom", TbDateFrom.Text},
	        {"DateTo", TbDateTo.Text}
	    });
	}
}
