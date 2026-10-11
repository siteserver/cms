using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Tasks
{
    public partial class TasksController
    {
        [HttpPost, Route(Route), Route(LegacyRoute)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

#pragma warning disable CS0618
            if (request.TaskType == TaskType.CloudSync || request.TaskType == TaskType.CloudBackup)
#pragma warning restore CS0618
            {
                return this.Error("云助手已移除，不支持该任务类型");
            }

            var task = new ScheduledTask();
            if (request.Id > 0)
            {
                task = await _scheduledTaskRepository.GetAsync(request.Id);
                if (task == null) return NotFound();
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.AdminId = _authManager.AdminId;
            task.TaskType = request.TaskType.GetValue();
            task.TaskInterval = request.TaskInterval;
            task.Every = request.Every;
            task.Weeks = request.Weeks;
            task.StartDate = request.StartDate;
            task.IsNoticeSuccess = request.IsNoticeSuccess;
            task.IsNoticeFailure = request.IsNoticeFailure;
            task.NoticeFailureCount = request.NoticeFailureCount;
            task.IsNoticeMobile = request.IsNoticeMobile;
            task.NoticeMobile = request.NoticeMobile;
            task.IsNoticeMail = request.IsNoticeMail;
            task.NoticeMail = request.NoticeMail;
            task.IsDisabled = request.IsDisabled;
            task.Timeout = request.Timeout;

            if (StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.Create.GetValue()))
            {
                task.CreateSiteIds = request.CreateSiteIds;
                task.CreateType = request.CreateType;
            }
            else if (StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.Ping.GetValue()))
            {
                task.PingHost = request.PingHost;
            }

            if (request.Id > 0)
            {
                await _scheduledTaskRepository.UpdateAsync(task);
            }
            else
            {
                await _scheduledTaskRepository.InsertAsync(task);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
