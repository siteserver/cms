using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageLogError : BasePage
	{
	    public DropDownList DdlCategory;
        public DropDownList DdlPluginId;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;
        public TextBox TbKeyword;
        public Repeater RptContents;
        public SqlPager SpContents;
		public Button BtnDelete;
		public Button BtnDeleteAll;
        public Literal LtlState;
        public Button BtnSetting;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var list = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("IDCollection"));
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
            else if (AuthRequest.IsQueryExists("DeleteAll"))
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
            else if (AuthRequest.IsQueryExists("Setting"))
            {
                ConfigManager.SystemConfigInfo.IsLogError = !ConfigManager.SystemConfigInfo.IsLogError;
                DataProvider.ConfigDao.Update(ConfigManager.Instance);
                SuccessMessage($"成功{(ConfigManager.SystemConfigInfo.IsLogError ? "启用" : "禁用")}日志记录");
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            SpContents.SelectCommand = DataProvider.ErrorLogDao.GetSelectCommend(AuthRequest.GetQueryString("category"), AuthRequest.GetQueryString("pluginId"), AuthRequest.GetQueryString("keyword"),
                    AuthRequest.GetQueryString("dateFrom"), AuthRequest.GetQueryString("dateTo"));

            SpContents.SortField = nameof(ErrorLogInfo.Id);
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            DdlCategory.Items.Add(new ListItem("全部", string.Empty));
            foreach (var category in LogUtils.AllCategoryList.Value)
            {
                DdlCategory.Items.Add(new ListItem(category.Value, category.Key));
            }

            DdlPluginId.Items.Add(new ListItem("全部", string.Empty));
            foreach (var pluginInfo in PluginManager.AllPluginInfoList)
            {
                DdlPluginId.Items.Add(new ListItem(pluginInfo.Id, pluginInfo.Id));
            }

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Log);

            if (AuthRequest.IsQueryExists("keyword"))
            {
                ControlUtils.SelectSingleItem(DdlCategory, AuthRequest.GetQueryString("category"));
                ControlUtils.SelectSingleItem(DdlPluginId, AuthRequest.GetQueryString("pluginId"));
                TbKeyword.Text = AuthRequest.GetQueryString("keyword");
                TbDateFrom.Text = AuthRequest.GetQueryString("dateFrom");
                TbDateTo.Text = AuthRequest.GetQueryString("dateTo");
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

            if (ConfigManager.SystemConfigInfo.IsLogError)
            {
                BtnSetting.Text = "禁用系统错误日志";
                BtnSetting.Attributes.Add("onclick",
                    AlertUtils.ConfirmRedirect("禁用系统错误日志", "此操作将禁用系统错误日志记录功能，确定吗？", "禁 用",
                        PageUtils.GetSettingsUrl(nameof(PageLogError), new NameValueCollection
                        {
                            {"Setting", "True"}
                        })));
            }
            else
            {
                LtlState.Text = @"<div class=""alert alert-danger m-t-10"">系统错误日志当前处于禁用状态，系统将不会记录系统错误日志！</div>";
                BtnSetting.Text = "启用系统错误日志";
                BtnSetting.Attributes.Add("onclick",
                    AlertUtils.ConfirmRedirect("启用系统错误日志", "此操作将启用系统错误日志记录功能，确定吗？", "启 用",
                        PageUtils.GetSettingsUrl(nameof(PageLogError), new NameValueCollection
                        {
                            {"Setting", "True"}
                        })));
            }

            SpContents.DataBind();
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var id = SqlUtils.EvalInt(e.Item.DataItem, nameof(ErrorLogInfo.Id));
            var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, nameof(ErrorLogInfo.AddDate));
            var message = SqlUtils.EvalString(e.Item.DataItem, nameof(ErrorLogInfo.Message));
            var summary = SqlUtils.EvalString(e.Item.DataItem, nameof(ErrorLogInfo.Summary));

            var ltlId = (Literal)e.Item.FindControl("ltlId");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlMessage = (Literal)e.Item.FindControl("ltlMessage");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

            ltlId.Text = $@"<a href=""{PageUtils.GetErrorPageUrl(id)}"" target=""_blank"">{id}</a>";
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(addDate);
            ltlMessage.Text = message;
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
	            {"category", DdlCategory.SelectedValue},
	            {"pluginId", DdlPluginId.SelectedValue},
	            {"keyword", TbKeyword.Text},
	            {"dateFrom", TbDateFrom.Text},
	            {"dateTo", TbDateTo.Text}
	        }));
	    }
	}
}
