using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageLogAdmin : BasePage
    {
        public Literal LtlState;
        public TextBox TbUserName;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;

		public Button BtnDelete;
		public Button BtnDeleteAll;
        public Button BtnSetting;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            SpContents.SelectCommand = !Body.IsQueryExists("Keyword") ? BaiRongDataProvider.LogDao.GetSelectCommend() : BaiRongDataProvider.LogDao.GetSelectCommend(Body.GetQueryString("UserName"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));

            SpContents.SortField = "ID";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
			{
                BreadCrumbSettings("管理员日志", AppManager.Permissions.Settings.Log);

                if (Body.IsQueryExists("Keyword"))
                {
                    TbUserName.Text = Body.GetQueryString("UserName");
                    TbKeyword.Text = Body.GetQueryString("Keyword");
                    TbDateFrom.Text = Body.GetQueryString("DateFrom");
                    TbDateTo.Text = Body.GetQueryString("DateTo");
                }

                if (Body.IsQueryExists("Delete"))
                {
                    var arraylist = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                    try
                    {
                        BaiRongDataProvider.LogDao.Delete(arraylist);
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
                        BaiRongDataProvider.LogDao.DeleteAll();
                        SuccessDeleteMessage();
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                }
                else if (Body.IsQueryExists("Setting"))
                {
                    try
                    {
                        ConfigManager.SystemConfigInfo.IsLogAdmin = !ConfigManager.SystemConfigInfo.IsLogAdmin;
                        BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
                        SuccessMessage($"成功{(ConfigManager.SystemConfigInfo.IsLogAdmin ? "启用" : "禁用")}日志记录");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, $"{(ConfigManager.SystemConfigInfo.IsLogAdmin ? "启用" : "禁用")}日志记录失败");
                    }
                }

                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetSettingsUrl(nameof(PageLogAdmin), new NameValueCollection
                {
                    {"Delete", "True" }
                }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

                BtnDeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.GetSettingsUrl(nameof(PageLogAdmin), new NameValueCollection
                {
                    {"DeleteAll", "True" }
                }), "此操作将删除所有日志信息，确定吗？"));

                if (ConfigManager.SystemConfigInfo.IsLogAdmin)
                {
                    BtnSetting.Text = "禁用记录日志功能";
                    BtnSetting.Attributes.Add("onclick",
                        PageUtils.GetRedirectStringWithConfirm(
                            PageUtils.GetSettingsUrl(nameof(PageLogAdmin), new NameValueCollection
                            {
                                {"Setting", "True"}
                            }), "此操作将禁用管理员日志记录功能，确定吗？"));
                }
                else
                {
                    LtlState.Text = " (管理员日志当前处于禁用状态，将不会记录相关操作！)";
                    BtnSetting.Text = "启用记录日志功能";
                    BtnSetting.Attributes.Add("onclick",
                        PageUtils.GetRedirectStringWithConfirm(
                            PageUtils.GetSettingsUrl(nameof(PageLogAdmin), new NameValueCollection
                            {
                                {"Setting", "True"}
                            }), "此操作将启用管理员日志记录功能，确定吗？"));
                }

                SpContents.DataBind();
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
                var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
                var ltlIpAddress = (Literal)e.Item.FindControl("ltlIpAddress");
                var ltlAction = (Literal)e.Item.FindControl("ltlAction");
                var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

                ltlUserName.Text = SqlUtils.EvalString(e.Item.DataItem, "UserName");
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"));
                ltlIpAddress.Text = SqlUtils.EvalString(e.Item.DataItem, "IPAddress");
                ltlAction.Text = SqlUtils.EvalString(e.Item.DataItem, "Action");
                ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, "Summary");
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

	    private string PageUrl => PageUtils.GetSettingsUrl(nameof(PageLogAdmin), new NameValueCollection
	    {
	        {"UserName", TbUserName.Text},
	        {"Keyword", TbKeyword.Text},
	        {"DateFrom", TbDateFrom.Text},
	        {"DateTo", TbDateTo.Text}
	    });
	}
}
