using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageServiceTask : BasePage
    {
        public Repeater RptContents;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete"))
            {
                var taskId = Body.GetQueryInt("TaskID");

                DataProvider.TaskDao.Delete(taskId);
                SuccessDeleteMessage();
            }

            if (Body.IsQueryExists("Enabled"))
            {
                var taskId = Body.GetQueryInt("TaskID");
                var isEnabled = Body.GetQueryBool("IsEnabled");
                var func = isEnabled ? "启用" : "禁用";

                DataProvider.TaskDao.UpdateState(taskId, isEnabled);
                SuccessMessage($"{func}定时任务成功。");
            }

			if (!IsPostBack)
			{
                VerifyAdministratorPermissions(AppManager.Permissions.Settings.Service);

                RptContents.DataSource = DataProvider.TaskDao.GetTaskInfoList();
                RptContents.ItemDataBound += RptContents_ItemDataBound;
                RptContents.DataBind();
			}
		}

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var taskInfo = (TaskInfo) e.Item.DataItem;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(taskInfo.PublishmentSystemId);
            if (publishmentSystemInfo != null)
            {
                var ltlPublishmentSystem = (Literal)e.Item.FindControl("ltlPublishmentSystem");
                var ltlTaskName = (Literal)e.Item.FindControl("ltlTaskName");
                var ltlServiceType = (Literal)e.Item.FindControl("ltlServiceType");
                var ltlIsEnabled = (Literal)e.Item.FindControl("ltlIsEnabled");
                var ltlFrequencyType = (Literal)e.Item.FindControl("ltlFrequencyType");
                var ltlLastExecuteDate = (Literal)e.Item.FindControl("ltlLastExecuteDate");
                var ltlEnabledHtml = (Literal)e.Item.FindControl("ltlEnabledHtml");
                var ltlDeleteHtml = (Literal)e.Item.FindControl("ltlDeleteHtml");

                ltlPublishmentSystem.Text = publishmentSystemInfo.PublishmentSystemName;

                ltlTaskName.Text = taskInfo.TaskName;
                ltlServiceType.Text = EServiceTypeUtils.GetText(taskInfo.ServiceType);
                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(taskInfo.IsEnabled.ToString());
                ltlFrequencyType.Text = EFrequencyTypeUtils.GetText(taskInfo.FrequencyType);
                if (taskInfo.LastExecuteDate > DateUtils.SqlMinValue)
                {
                    ltlLastExecuteDate.Text = DateUtils.GetDateAndTimeString(taskInfo.LastExecuteDate);
                }

                var urlTask = PageUtils.GetSettingsUrl(nameof(PageServiceTask), new NameValueCollection
            {
                {"Enabled", "True"},
                {"TaskID", taskInfo.TaskId.ToString()},
                {"IsEnabled", (!taskInfo.IsEnabled).ToString() }
            });
                ltlEnabledHtml.Text =
                    $"<a href=\"{urlTask}\" onClick=\"javascript:return confirm('此操作将{(taskInfo.IsEnabled ? "禁用" : "启用")}任务“{taskInfo.TaskName}”，确认吗？');\">{(taskInfo.IsEnabled ? "禁用" : "启用")}</a>";

                if (!taskInfo.IsSystemTask)
                {
                    var urlDelete = PageUtils.GetSettingsUrl(nameof(PageServiceTask), new NameValueCollection
                {
                    {"Delete", "True"},
                    {"TaskID", taskInfo.TaskId.ToString()},
                });

                    ltlDeleteHtml.Text =
                        $"<a href=\"{urlDelete}\" onClick=\"javascript:return confirm('此操作将删除任务“{taskInfo.TaskName}”，确认吗？');\">删除</a>";
                }
            }
            else
            {
                e.Item.Visible = false;
            }
        }
	}
}
