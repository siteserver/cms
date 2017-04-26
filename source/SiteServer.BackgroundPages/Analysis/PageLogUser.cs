using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageLogUser : BasePage
    {
        public Literal ltlState;
        public TextBox UserName;
        public TextBox Keyword;
        public DateTimeTextBox DateFrom;
        public DateTimeTextBox DateTo;

        public Repeater rptContents;
        public SqlPager spContents;

		public Button Delete;
		public Button DeleteAll;
        public Button Setting;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = StringUtils.Constants.PageSize;

            if (!Body.IsQueryExists("Keyword"))
            {
                spContents.SelectCommand = BaiRongDataProvider.UserLogDao.GetSelectCommend();
            }
            else
            {
                spContents.SelectCommand = BaiRongDataProvider.UserLogDao.GetSelectCommend(Body.GetQueryString("UserName"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));
            }

            spContents.SortField = "ID";
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
			{
                BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Log, "用户日志", AppManager.Analysis.Permission.AnalysisLog);

                if (Body.IsQueryExists("Keyword"))
                {
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
                        BaiRongDataProvider.UserLogDao.Delete(arraylist);
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
                        BaiRongDataProvider.UserLogDao.DeleteAll();
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
                        ConfigManager.SystemConfigInfo.IsLogUser = !ConfigManager.SystemConfigInfo.IsLogUser;
                        BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
                        SuccessMessage($"成功{(ConfigManager.SystemConfigInfo.IsLogUser ? "启用" : "禁用")}日志记录");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, $"{(ConfigManager.SystemConfigInfo.IsLogUser ? "启用" : "禁用")}日志记录失败");
                    }
                }

                Delete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetAnalysisUrl(nameof(PageLogUser), new NameValueCollection
                {
                    {"Delete", "True" }
                }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

                DeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.GetAnalysisUrl(nameof(PageLogUser), new NameValueCollection
                {
                    {"DeleteAll", "True" }
                }), "此操作将删除所有日志信息，确定吗？"));

                if (ConfigManager.SystemConfigInfo.IsLogUser)
                {
                    Setting.Text = "禁用记录日志功能";
                    Setting.Attributes.Add("onclick",
                        PageUtils.GetRedirectStringWithConfirm(
                            PageUtils.GetAnalysisUrl(nameof(PageLogUser), new NameValueCollection
                            {
                                {"Setting", "True"}
                            }), "此操作将禁用用户日志记录功能，确定吗？"));
                }
                else
                {
                    ltlState.Text = " (用户日志当前处于禁用状态，将不会记录相关操作！)";
                    Setting.Text = "启用记录日志功能";
                    Setting.Attributes.Add("onclick",
                        PageUtils.GetRedirectStringWithConfirm(
                            PageUtils.GetAnalysisUrl(nameof(PageLogUser), new NameValueCollection
                            {
                                {"Setting", "True"}
                            }), "此操作将启用用户日志记录功能，确定吗？"));
                }

                spContents.DataBind(); 
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
                var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
                var ltlIpAddress = (Literal)e.Item.FindControl("ltlIPAddress");
                var ltlAction = (Literal)e.Item.FindControl("ltlAction");
                var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");

                ltlUserName.Text = SqlUtils.EvalString(e.Item.DataItem, "UserName");
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"));
                ltlIpAddress.Text = SqlUtils.EvalString(e.Item.DataItem, "IPAddress");
                ltlAction.Text = EUserActionTypeUtils.GetText(EUserActionTypeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "Action")));
                ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, "Summary");
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

	    private string PageUrl => PageUtils.GetAnalysisUrl(nameof(PageLogUser), new NameValueCollection
	    {
	        {"UserName", UserName.Text},
	        {"Keyword", Keyword.Text},
	        {"DateFrom", DateFrom.Text},
	        {"DateTo", DateTo.Text}
	    });
	}
}
