using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class SettingsBackupController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var tasks = await _scheduledTaskRepository.GetAllAsync();
            var cloudSyncs = tasks.Where(task => task.TaskType == TaskType.CloudSync).ToList();
            if (request.IsCloudBackup)
            {
                if (cloudSyncs.Count >= 1)
                {
                    for (int i = 0; i < cloudSyncs.Count; i++)
                    {
                        if (i == 0) continue;
                        await _scheduledTaskRepository.DeleteAsync(cloudSyncs[i].Id);
                    }
                }
                else
                {
                    await _scheduledTaskRepository.InsertCloudSyncAsync();
                }
            }
            else
            {
                for (int i = 0; i < cloudSyncs.Count; i++)
                {
                    await _scheduledTaskRepository.DeleteAsync(cloudSyncs[i].Id);
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
