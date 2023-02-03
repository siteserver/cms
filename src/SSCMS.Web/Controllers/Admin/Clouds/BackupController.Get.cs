using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class BackupController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var cloudType = await _cloudManager.GetCloudTypeAsync();
            var config = await _configRepository.GetAsync();
            var isConfig = config.IsCloudBackup;
            
            var isTask = false;
            var tasks = await _scheduledTaskRepository.GetAllAsync();
            var cloudBackups = tasks.Where(task => task.TaskType == TaskType.CloudBackup).ToList();
            if (cloudBackups.Count >= 1)
            {
                isTask = true;
                for (int i = 0; i < cloudBackups.Count; i++)
                {
                    if (i == 0) continue;
                    await _scheduledTaskRepository.DeleteAsync(cloudBackups[i].Id);
                }
            }

            return new GetResult
            {
                CloudType = cloudType,
                IsCloudBackup = isConfig && isTask,
            };
        }
    }
}
