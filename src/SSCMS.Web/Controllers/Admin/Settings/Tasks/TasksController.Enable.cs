using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Tasks
{
    public partial class TasksController
    {
        [HttpPost, Route(RouteEnable), Route(LegacyRouteEnable)]
        public async Task<ActionResult<BoolResult>> Enable([FromBody] IdRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var task = await _scheduledTaskRepository.GetAsync(request.Id);
            if (task == null) return NotFound();
#pragma warning disable CS0618
            var isRemovedType = StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.CloudSync.GetValue()) ||
                                StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.CloudBackup.GetValue());
#pragma warning restore CS0618
            if (isRemovedType && task.IsDisabled)
            {
                return this.Error("云助手已移除，该任务无法启用");
            }
            task.IsDisabled = !task.IsDisabled;
            await _scheduledTaskRepository.UpdateAsync(task);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
