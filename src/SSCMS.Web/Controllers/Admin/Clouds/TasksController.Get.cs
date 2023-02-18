using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class TasksController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var cloudType = await _cloudManager.GetCloudTypeAsync();
            var taskTypes = ListUtils.GetSelects<TaskType>();
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
                CloudType = cloudType,
                TaskTypes = taskTypes,
                TaskIntervals = taskIntervals,
                Tasks = tasks,
                Sites = sites,
            };
        }
    }
}
