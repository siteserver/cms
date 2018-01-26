using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageLogError : BasePage
	{
	    public DropDownList DdlPluginId;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;
        public TextBox TbKeyword;
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
                    DataProvider.ErrorLogDao.Delete(list);
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
                    DataProvider.ErrorLogDao.DeleteAll();
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            SpContents.SelectCommand = DataProvider.ErrorLogDao.GetSelectCommend(Body.GetQueryString("PluginId"), Body.GetQueryString("Keyword"),
                    Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));

            SpContents.SortField = nameof(ErrorLogInfo.Id);
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            DdlPluginId.Items.Add(new ListItem("全部错误", string.Empty));
            foreach (var pluginInfo in PluginManager.AllPluginInfoList)
            {
                DdlPluginId.Items.Add(new ListItem(pluginInfo.Id, pluginInfo.Id));
            }

            VerifyAdministratorPermissions(ConfigManager.Permissions.Settings.Log);

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

            BtnDeleteAll.Attributes.Add("onclick",
                AlertUtils.ConfirmRedirect("删除所有日志", "此操作将删除所有日志信息，确定吗？", "删除全部",
                    PageUtils.GetSettingsUrl(nameof(PageLogError), new NameValueCollection
                    {
                        {"DeleteAll", "True"}
                    })));

            SpContents.DataBind();
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var id = SqlUtils.EvalInt(e.Item.DataItem, nameof(ErrorLogInfo.Id));
            var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, nameof(ErrorLogInfo.AddDate));
            var message = SqlUtils.EvalString(e.Item.DataItem, nameof(ErrorLogInfo.Message));
            var stacktrace = SqlUtils.EvalString(e.Item.DataItem, nameof(ErrorLogInfo.Stacktrace));
            var summary = SqlUtils.EvalString(e.Item.DataItem, nameof(ErrorLogInfo.Summary));

            var ltlId = (Literal)e.Item.FindControl("ltlId");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlMessage = (Literal)e.Item.FindControl("ltlMessage");
            var ltlStacktrace = (Literal)e.Item.FindControl("ltlStacktrace");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

            ltlId.Text = id.ToString();
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(addDate);
            ltlMessage.Text = message;
            ltlStacktrace.Text = stacktrace;
            ltlSummary.Text = summary;
            if (!string.IsNullOrEmpty(ltlSummary.Text))
            {
                ltlSummary.Text += "<br />";
            }
        }

	    public void Search_OnClick(object sender, EventArgs e)
	    {
	        PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageLogError), new NameValueCollection
	        {
	            {"PluginId", DdlPluginId.SelectedValue},
	            {"Keyword", TbKeyword.Text},
	            {"DateFrom", TbDateFrom.Text},
	            {"DateTo", TbDateTo.Text}
	        }));
	    }
	}
}
