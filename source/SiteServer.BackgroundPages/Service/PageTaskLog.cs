using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Service
{
    public class PageTaskLog : BasePage
    {
        public Literal ltlState;
        public DropDownList ddlIsSuccess;
        public TextBox tbKeyword;
        public DateTimeTextBox tbDateFrom;
        public DateTimeTextBox tbDateTo;

        public Repeater rptContents;
        public SqlPager spContents;

		public Button btnDelete;
		public Button btnDeleteAll;
        public Button btnSetting;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = StringUtils.Constants.PageSize;

            if (Body.GetQueryString("Keyword") == null)
            {
                spContents.SelectCommand = DataProvider.TaskLogDao.GetSelectCommend();
            }
            else
            {
                spContents.SelectCommand = DataProvider.TaskLogDao.GetSelectCommend(ETriStateUtils.GetEnumType(Body.GetQueryString("IsSuccess")), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));
            }

            spContents.SortField = "ID";
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
            {
                BreadCrumbService(AppManager.Service.LeftMenu.Task, "任务运行日志", AppManager.Service.Permission.ServiceTask);

                ETriStateUtils.AddListItems(ddlIsSuccess, "全部", "成功", "失败");

                if (Body.IsQueryExists("Keyword"))
                {
                    ControlUtils.SelectListItems(ddlIsSuccess, Body.GetQueryString("IsSuccess"));
                    tbKeyword.Text = Body.GetQueryString("Keyword");
                    tbDateFrom.Text = Body.GetQueryString("DateFrom");
                    tbDateTo.Text = Body.GetQueryString("DateTo");
                }

                if (Body.IsQueryExists("Delete"))
                {
                    var arraylist = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                    try
                    {
                        DataProvider.TaskLogDao.Delete(arraylist);
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
                        DataProvider.TaskLogDao.DeleteAll();
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
                        ConfigManager.SystemConfigInfo.IsLogTask = !ConfigManager.SystemConfigInfo.IsLogTask;
                        BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
                        SuccessMessage($"成功{(ConfigManager.SystemConfigInfo.IsLogTask ? "启用" : "禁用")}日志记录");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, $"{(ConfigManager.SystemConfigInfo.IsLogTask ? "启用" : "禁用")}日志记录失败");
                    }
                }

                var deleteUrl = PageUtils.GetServiceUrl(nameof(PageTaskLog), new NameValueCollection
                {
                    {"Delete", "True"},
                });
                var deleteAllUrl = PageUtils.GetServiceUrl(nameof(PageTaskLog), new NameValueCollection
                {
                    {"DeleteAll", "True"},
                });

                btnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(deleteUrl, "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));
                btnDeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(deleteAllUrl, "此操作将删除所有日志信息，确定吗？"));

                var settingUrl = PageUtils.GetServiceUrl(nameof(PageTaskLog), new NameValueCollection
                {
                    {"Setting", "True"},
                });

                if (ConfigManager.SystemConfigInfo.IsLogTask)
                {
                    btnSetting.Text = "禁用记录日志功能";
                    btnSetting.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(settingUrl, "此操作将禁用任务运行日志记录功能，确定吗？"));
                }
                else
                {
                    ltlState.Text = " (任务运行日志当前处于禁用状态，将不会记录相关操作！)";
                    btnSetting.Text = "启用记录日志功能";
                    btnSetting.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(settingUrl, "此操作将启用任务运行日志记录功能，确定吗？"));
                }

                spContents.DataBind();
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var taskID = SqlUtils.EvalInt(e.Item.DataItem, "TaskID");
                var taskInfo = DataProvider.TaskDao.GetTaskInfo(taskID);

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(taskInfo.PublishmentSystemID);
                if (publishmentSystemInfo == null)
                {
                    e.Item.Visible = false;
                }

                var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate");
                var isSuccess = SqlUtils.EvalString(e.Item.DataItem, "IsSuccess");
                var errorMessage = SqlUtils.EvalString(e.Item.DataItem, "ErrorMessage");

                var ltlPublishmentSystem = (Literal)e.Item.FindControl("ltlPublishmentSystem");
                var ltlTaskName = (Literal)e.Item.FindControl("ltlTaskName");
                var ltlServiceType = (Literal)e.Item.FindControl("ltlServiceType");
                var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
                var ltlIsSuccess = (Literal)e.Item.FindControl("ltlIsSuccess");
                var ltlErrorMessage = (Literal)e.Item.FindControl("ltlErrorMessage");

                ltlPublishmentSystem.Text = publishmentSystemInfo.PublishmentSystemName;
                ltlTaskName.Text = taskInfo.TaskName;
                ltlServiceType.Text = EServiceTypeUtils.GetText(taskInfo.ServiceType);
                
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(addDate);
                ltlIsSuccess.Text = StringUtils.GetTrueOrFalseImageHtml(isSuccess);
                ltlErrorMessage.Text = errorMessage;
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

	    private string PageUrl => PageUtils.GetServiceUrl(nameof(PageTaskLog), new NameValueCollection
	    {
	        {"Keyword", tbKeyword.Text},
	        {"DateFrom", tbDateFrom.Text},
	        {"DateTo", tbDateTo.Text},
	        {"IsSuccess", ddlIsSuccess.SelectedValue}
	    });
	}
}
