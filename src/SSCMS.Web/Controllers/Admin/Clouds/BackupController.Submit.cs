using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class BackupController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var tasks = await _scheduledTaskRepository.GetAllAsync();
            var cloudBackups = tasks.Where(task => task.TaskType == TaskType.CloudBackup).ToList();
            if (request.IsCloudBackup)
            {
                if (cloudBackups.Count >= 1)
                {
                    for (int i = 0; i < cloudBackups.Count; i++)
                    {
                        if (i == 0) continue;
                        await _scheduledTaskRepository.DeleteAsync(cloudBackups[i].Id);
                    }
                }
                else
                {
                    await _scheduledTaskRepository.InsertCloudBackupAsync();
                }
            }
            else
            {
                for (int i = 0; i < cloudBackups.Count; i++)
                {
                    await _scheduledTaskRepository.DeleteAsync(cloudBackups[i].Id);
                }
            }

            var config = await _configRepository.GetAsync();

            config.IsCloudBackup = request.IsCloudBackup;
            await _configRepository.UpdateAsync(config);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
