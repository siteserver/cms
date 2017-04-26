using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTask : BasePageCms
    {
        public DataGrid dgContents;
        public Button AddTask;

        private EServiceType _serviceType;

        public static string GetRedirectUrl(int publishmentSystemId, EServiceType serviceType)
        {
            return PageUtils.GetCmsUrl(nameof(PageTask), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ServiceType", EServiceTypeUtils.GetValue(serviceType)}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("ServiceType");

            _serviceType = EServiceTypeUtils.GetEnumType(Body.GetQueryString("ServiceType"));

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
                var taskName = EServiceTypeUtils.GetText(_serviceType);

                if (PublishmentSystemId > 0)
                {
                    BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationTask, taskName + "任务", AppManager.Cms.Permission.WebSite.Configration);
                }
                else
                {
                    BreadCrumbService(AppManager.Service.LeftMenu.Task, taskName + "任务", AppManager.Service.Permission.ServiceTask);
                }

                AddTask.Text = $"添加{taskName}任务";
                AddTask.Attributes.Add("onclick", ModalTaskAdd.GetOpenWindowStringToAdd(_serviceType, PublishmentSystemId));
                BindGrid();
			}
		}

        public void BindGrid()
        {
            try
            {
                if (PublishmentSystemId != 0)
                {
                    dgContents.DataSource = DataProvider.TaskDao.GetTaskInfoList(_serviceType, PublishmentSystemId);
                    dgContents.Columns.RemoveAt(0);
                }
                else
                {
                    dgContents.DataSource = DataProvider.TaskDao.GetTaskInfoList(_serviceType);
                }
                dgContents.ItemDataBound += DgContents_ItemDataBound;
                dgContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var taskInfo = (TaskInfo)e.Item.DataItem;

            var ltlSite = (Literal)e.Item.FindControl("ltlSite");
            var ltlTaskName = (Literal)e.Item.FindControl("ltlTaskName");
            var ltlIsEnabled = (Literal)e.Item.FindControl("ltlIsEnabled");
            var ltlFrequencyType = (Literal)e.Item.FindControl("ltlFrequencyType");
            var ltlLastExecuteDate = (Literal)e.Item.FindControl("ltlLastExecuteDate");
            var ltlEditHtml = (Literal)e.Item.FindControl("ltlEditHtml");
            var ltlEnabledHtml = (Literal)e.Item.FindControl("ltlEnabledHtml");
            var ltlDeleteHtml = (Literal)e.Item.FindControl("ltlDeleteHtml");

            if (ltlSite != null)
            {
                ltlSite.Text = GetSiteHtml(taskInfo.PublishmentSystemID);
            }
            ltlTaskName.Text = taskInfo.TaskName;
            ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(taskInfo.IsEnabled.ToString());
            ltlFrequencyType.Text = EFrequencyTypeUtils.GetText(taskInfo.FrequencyType);
            if (taskInfo.LastExecuteDate > DateUtils.SqlMinValue)
            {
                ltlLastExecuteDate.Text = DateUtils.GetDateAndTimeString(taskInfo.LastExecuteDate);
            }
            if (!taskInfo.IsSystemTask)
            {
                ltlEditHtml.Text = GetEditHtml(taskInfo.TaskID, taskInfo.PublishmentSystemID);
                ltlDeleteHtml.Text = GetDeleteHtml(taskInfo.TaskID, taskInfo.TaskName, taskInfo.PublishmentSystemID);
            }
            var urlTask = PageUtils.GetCmsUrl(nameof(PageTask), new NameValueCollection
            {
                {"PublishmentSystemID", taskInfo.PublishmentSystemID.ToString()},
                {"TaskID", taskInfo.TaskID.ToString()},
                {"ServiceType", EServiceTypeUtils.GetValue(taskInfo.ServiceType)},
                {"Enabled", true.ToString()},
                {"IsEnabled", (!taskInfo.IsEnabled).ToString()}
            });
            ltlEnabledHtml.Text =
                $"<a href=\"{urlTask}\" onClick=\"javascript:return confirm('此操作将{(taskInfo.IsEnabled ? "禁用" : "启用")}任务“{taskInfo.TaskName}”，确认吗？');\">{(taskInfo.IsEnabled ? "禁用" : "启用")}</a>";
        }

        private string GetEditHtml(int taskId, int publishmentSystemId)
        {
            if (PublishmentSystemId != 0 && publishmentSystemId != PublishmentSystemId) return string.Empty;
            return
                $"<a href=\"javascript:;\" onClick=\"{ModalTaskAdd.GetOpenWindowStringToEdit(taskId, _serviceType, PublishmentSystemId)}\">修改</a>";
        }

        private string GetSiteHtml(int publishmentSystemId)
        {
            if (publishmentSystemId == 0) return "全部站点";
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo != null)
            {
                return
                    $"<a href=\"{publishmentSystemInfo.PublishmentSystemUrl}\" target=\"_blank\">{publishmentSystemInfo.PublishmentSystemName}</a>";
            }
            return string.Empty;
        }

        private string GetDeleteHtml(int taskId, string taskName, int publishmentSystemId)
        {
            if (PublishmentSystemId != 0 && publishmentSystemId != PublishmentSystemId) return string.Empty;

            var urlDelete = PageUtils.GetCmsUrl(nameof(PageTask), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"TaskID", taskId.ToString()},
                    {"ServiceType", EServiceTypeUtils.GetValue(_serviceType)},
                    {"Delete", true.ToString()}
                });
            return
                $"<a href=\"{urlDelete}\" onClick=\"javascript:return confirm('此操作将删除{EServiceTypeUtils.GetText(_serviceType)}任务“{taskName}”，确认吗？');\">删除</a>";
        }
	}
}
