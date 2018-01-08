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

namespace SiteServer.BackgroundPages.Settings
{
    public class PageLogServiceTask : BasePage
    {
        public DropDownList DdlIsSuccess;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;
        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlState;
        public Button BtnDelete;
		public Button BtnDeleteAll;
        public Button BtnSetting;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;

            SpContents.SelectCommand = Body.GetQueryString("Keyword") == null ? DataProvider.TaskLogDao.GetSelectCommend() : DataProvider.TaskLogDao.GetSelectCommend(ETriStateUtils.GetEnumType(Body.GetQueryString("IsSuccess")), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));

            SpContents.SortField = "ID";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Log);

            ETriStateUtils.AddListItems(DdlIsSuccess, "全部", "成功", "失败");

            if (Body.IsQueryExists("Keyword"))
            {
                ControlUtils.SelectSingleItem(DdlIsSuccess, Body.GetQueryString("IsSuccess"));
                TbKeyword.Text = Body.GetQueryString("Keyword");
                TbDateFrom.Text = Body.GetQueryString("DateFrom");
                TbDateTo.Text = Body.GetQueryString("DateTo");
            }

            if (Body.IsQueryExists("Delete"))
            {
                var list = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                DataProvider.TaskLogDao.Delete(list);
                SuccessDeleteMessage();
            }
            else if (Body.IsQueryExists("DeleteAll"))
            {
                DataProvider.TaskLogDao.DeleteAll();
                SuccessDeleteMessage();
            }
            else if (Body.IsQueryExists("Setting"))
            {
                ConfigManager.SystemConfigInfo.IsLogTask = !ConfigManager.SystemConfigInfo.IsLogTask;
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
                SuccessMessage($"成功{(ConfigManager.SystemConfigInfo.IsLogTask ? "启用" : "禁用")}日志记录");
            }

            BtnDelete.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetSettingsUrl(nameof(PageLogServiceTask), new NameValueCollection
                    {
                        {"Delete", "True"}
                    }), "IDCollection", "IDCollection", "请选择需要删除的日志！", "此操作将删除所选日志，确认吗？"));

            BtnDeleteAll.Attributes.Add("onclick",
                AlertUtils.ConfirmRedirect("删除所有日志", "此操作将删除所有日志信息，确定吗？", "删除全部",
                    PageUtils.GetSettingsUrl(nameof(PageLogServiceTask), new NameValueCollection
                    {
                        {"DeleteAll", "True"}
                    })));

            if (ConfigManager.SystemConfigInfo.IsLogTask)
            {
                BtnSetting.Text = "禁用记录日志";
                BtnSetting.Attributes.Add("onclick",
                    AlertUtils.ConfirmRedirect("禁用记录日志", "此操作将禁用任务运行日志记录功能，确定吗？", "禁 用",
                        PageUtils.GetSettingsUrl(nameof(PageLogServiceTask), new NameValueCollection
                        {
                            {"Setting", "True"}
                        })));
            }
            else
            {
                LtlState.Text = @"<div class=""alert alert-danger m-t-10"">任务运行日志当前处于禁用状态，将不会记录相关操作！</div>";

                BtnSetting.Text = "启用记录日志";
                BtnSetting.Attributes.Add("onclick",
                    AlertUtils.ConfirmRedirect("启用记录日志", "此操作将启用任务运行日志记录功能，确定吗？", "启 用",
                        PageUtils.GetSettingsUrl(nameof(PageLogServiceTask), new NameValueCollection
                        {
                            {"Setting", "True"}
                        })));
            }

            SpContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var taskId = SqlUtils.EvalInt(e.Item.DataItem, "TaskID");
            var taskInfo = DataProvider.TaskDao.GetTaskInfo(taskId);

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(taskInfo.PublishmentSystemId);
            if (publishmentSystemInfo == null)
            {
                e.Item.Visible = false;
            }
            else
            {
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
            PageUtils.Redirect(PageUtils.GetSettingsUrl(nameof(PageLogServiceTask), new NameValueCollection
            {
                {"Keyword", TbKeyword.Text},
                {"DateFrom", TbDateFrom.Text},
                {"DateTo", TbDateTo.Text},
                {"IsSuccess", DdlIsSuccess.SelectedValue}
            }));
        }
    }
}
