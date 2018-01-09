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
	    public Literal LtlNavItems;
        public Repeater RptContents;
        public Button BtnAdd;

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

                DataProvider.TaskDao.UpdateState(taskId, isEnabled);
                SuccessMessage($"{func}定时任务成功。");
            }

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            LtlNavItems.Text = $@"
<li class=""nav-item {(_serviceType == EServiceType.Create ? "active": string.Empty)}"">
    <a class=""nav-link"" href=""{GetRedirectUrl(PublishmentSystemId, EServiceType.Create)}"">定时生成设置</a>
</li>
<li class=""nav-item {(_serviceType == EServiceType.Gather ? "active" : string.Empty)}"">
    <a class=""nav-link"" href=""{GetRedirectUrl(PublishmentSystemId, EServiceType.Gather)}"">定时采集设置</a>
</li>
<li class=""nav-item {(_serviceType == EServiceType.Backup ? "active" : string.Empty)}"">
    <a class=""nav-link"" href=""{GetRedirectUrl(PublishmentSystemId, EServiceType.Backup)}"">定时备份设置</a>
</li>";

            RptContents.DataSource = DataProvider.TaskDao.GetTaskInfoList(_serviceType, PublishmentSystemId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAdd.Text = $"添加{EServiceTypeUtils.GetText(_serviceType)}任务";
            BtnAdd.Attributes.Add("onclick", ModalTaskAdd.GetOpenWindowStringToAdd(_serviceType, PublishmentSystemId));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

            ltlSite.Text = GetSiteHtml(taskInfo.PublishmentSystemId);
            ltlTaskName.Text = taskInfo.TaskName;
            ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(taskInfo.IsEnabled.ToString());
            ltlFrequencyType.Text = EFrequencyTypeUtils.GetText(taskInfo.FrequencyType);
            if (taskInfo.LastExecuteDate > DateUtils.SqlMinValue)
            {
                ltlLastExecuteDate.Text = DateUtils.GetDateAndTimeString(taskInfo.LastExecuteDate);
            }
            if (!taskInfo.IsSystemTask)
            {
                ltlEditHtml.Text = GetEditHtml(taskInfo.TaskId, taskInfo.PublishmentSystemId);
                ltlDeleteHtml.Text = GetDeleteHtml(taskInfo.TaskId, taskInfo.TaskName, taskInfo.PublishmentSystemId);
            }
            var urlTask = PageUtils.GetCmsUrl(nameof(PageTask), new NameValueCollection
            {
                {"PublishmentSystemID", taskInfo.PublishmentSystemId.ToString()},
                {"TaskID", taskInfo.TaskId.ToString()},
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
                    $"<a href=\"{publishmentSystemInfo.Additional.WebUrl}\" target=\"_blank\">{publishmentSystemInfo.PublishmentSystemName}</a>";
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
