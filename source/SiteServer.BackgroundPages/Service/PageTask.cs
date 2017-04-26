using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Service
{
    public class PageTask : BasePage
    {
        public DataGrid dgContents;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete"))
            {
                var taskId = Body.GetQueryInt("TaskID");
                try
                {
                    DataProvider.TaskDao.Delete(taskId);
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (Body.IsQueryExists("Enabled"))
            {
                var taskId = Body.GetQueryInt("TaskID");
                var isEnabled = Body.GetQueryBool("IsEnabled");
                var func = isEnabled ? "启用" : "禁用";

                try
                {
                    DataProvider.TaskDao.UpdateState(taskId, isEnabled);
                    SuccessMessage($"{func}定时任务成功。");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"{func}定时任务失败。");
                }
            }

			if (!IsPostBack)
			{
                BreadCrumbService(AppManager.Service.LeftMenu.Task, "定时任务管理", AppManager.Service.Permission.ServiceTask);

                dgContents.DataSource = DataProvider.TaskDao.GetTaskInfoList();
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var taskInfo = (TaskInfo)e.Item.DataItem;

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(taskInfo.PublishmentSystemID);
                if (publishmentSystemInfo == null)
                {
                    e.Item.Visible = false;
                }

                var ltlPublishmentSystem = e.Item.FindControl("ltlPublishmentSystem") as Literal;
                var ltlTaskName = e.Item.FindControl("ltlTaskName") as Literal;
                var ltlServiceType = e.Item.FindControl("ltlServiceType") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlFrequencyType = e.Item.FindControl("ltlFrequencyType") as Literal;
                var ltlLastExecuteDate = e.Item.FindControl("ltlLastExecuteDate") as Literal;
                var ltlEnabledHtml = e.Item.FindControl("ltlEnabledHtml") as Literal;
                var ltlDeleteHtml = e.Item.FindControl("ltlDeleteHtml") as Literal;

                ltlPublishmentSystem.Text = publishmentSystemInfo.PublishmentSystemName;

                ltlTaskName.Text = taskInfo.TaskName;
                ltlServiceType.Text = EServiceTypeUtils.GetText(taskInfo.ServiceType);
                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(taskInfo.IsEnabled.ToString());
                ltlFrequencyType.Text = EFrequencyTypeUtils.GetText(taskInfo.FrequencyType);
                if (taskInfo.LastExecuteDate > DateUtils.SqlMinValue)
                {
                    ltlLastExecuteDate.Text = DateUtils.GetDateAndTimeString(taskInfo.LastExecuteDate);
                }

                var urlTask = PageUtils.GetServiceUrl(nameof(PageTask), new NameValueCollection
                {
                    {"Enabled", "True"},
                    {"TaskID", taskInfo.TaskID.ToString()},
                    {"IsEnabled", (!taskInfo.IsEnabled).ToString() }
                });
                ltlEnabledHtml.Text =
                    $"<a href=\"{urlTask}\" onClick=\"javascript:return confirm('此操作将{(taskInfo.IsEnabled ? "禁用" : "启用")}任务“{taskInfo.TaskName}”，确认吗？');\">{(taskInfo.IsEnabled ? "禁用" : "启用")}</a>";
                if (!taskInfo.IsSystemTask)
                {
                    var urlDelete = PageUtils.GetServiceUrl(nameof(PageTask), new NameValueCollection
                    {
                        {"Delete", "True"},
                        {"TaskID", taskInfo.TaskID.ToString()},
                    });

                    ltlDeleteHtml.Text =
                        $"<a href=\"{urlDelete}\" onClick=\"javascript:return confirm('此操作将删除任务“{taskInfo.TaskName}”，确认吗？');\">删除</a>";
                }
            }
        }
	}
}
