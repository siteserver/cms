using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageLogError : BasePage
	{
	    public DropDownList DdlPluginId;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;

		public Button BtnDelete;
		public Button BtnDeleteAll;

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

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            SpContents.SelectCommand = BaiRongDataProvider.ErrorLogDao.GetSelectCommend(Body.GetQueryString("PluginId"), Body.GetQueryString("Keyword"),
                    Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));

            SpContents.SortField = "Id";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            DdlPluginId.Items.Add(new ListItem("全部错误", string.Empty));
            foreach (var pair in PluginManager.AllPluginPairs)
            {
                DdlPluginId.Items.Add(new ListItem(pair.Metadata.DisplayName, pair.Metadata.Id));
            }

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Log);

            if (Body.IsQueryExists("Keyword"))
            {
                ControlUtils.SelectSingleItem(DdlPluginId, Body.GetQueryString("PluginId"));
                TbKeyword.Text = Body.GetQueryString("Keyword");
                TbDateFrom.Text = Body.GetQueryString("DateFrom");
                TbDateTo.Text = Body.GetQueryString("DateTo");
            }

            BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetSettingsUrl(nameof(PageLogError), new NameValueCollection
            {
                {"Delete", "True" }
            }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

            BtnDeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.GetSettingsUrl(nameof(PageLogError), new NameValueCollection
            {
                {"DeleteAll", "True" }
            }), "此操作将删除所有日志信息，确定吗？"));

            SpContents.DataBind();
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlMessage = (Literal)e.Item.FindControl("ltlMessage");
            var ltlStacktrace = (Literal)e.Item.FindControl("ltlStacktrace");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

            ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"));
            ltlMessage.Text = SqlUtils.EvalString(e.Item.DataItem, "Message");
            ltlStacktrace.Text = SqlUtils.EvalString(e.Item.DataItem, "Stacktrace");
            ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, "Summary");
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

	    private string PageUrl => PageUtils.GetSettingsUrl(nameof(PageLogError), new NameValueCollection
	    {
            {"PluginId", DdlPluginId.SelectedValue},
            {"Keyword", TbKeyword.Text},
	        {"DateFrom", TbDateFrom.Text},
	        {"DateTo", TbDateTo.Text}
	    });
	}
}
