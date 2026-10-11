using System.Collections.Generic;
using System.Linq;
using Datory;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Tasks
{
    public partial class TasksController
    {
        [HttpGet, Route(Route), Route(LegacyRoute)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            // 云同步/云备份已随云助手移除，不再提供选择
#pragma warning disable CS0618
            var removedTypes = new[] { TaskType.CloudSync.GetValue(), TaskType.CloudBackup.GetValue() };
#pragma warning restore CS0618
            var taskTypes = ListUtils.GetSelects<TaskType>().Where(x => !removedTypes.Contains(x.Value)).ToList();
            var taskIntervals = ListUtils.GetSelects<TaskInterval>();
            var tasks = await _scheduledTaskRepository.GetAllAsync();
            // tasks = tasks.Where(task => task.TaskType == TaskType.Create || task.TaskType == TaskType.Ping).ToList();

            var sites = new List<Select<int>>();
            foreach (var site in await _siteRepository.GetSitesAsync())
            {
                 sites.Add(new Select<int>(site.Id, site.SiteName));
            }

            return new GetResult
            {
                TaskTypes = taskTypes,
                TaskIntervals = taskIntervals,
                Tasks = tasks,
                Sites = sites,
            };
        }
    }
}
