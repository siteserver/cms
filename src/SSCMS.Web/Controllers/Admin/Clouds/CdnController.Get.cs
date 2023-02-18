using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class CdnController
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

            var isTask = false;
            var tasks = await _scheduledTaskRepository.GetAllAsync();
            var cloudSyncs = tasks.Where(task => task.TaskType == TaskType.CloudSync).ToList();
            if (cloudSyncs.Count >= 1)
            {
                isTask = true;
                for (int i = 0; i < cloudSyncs.Count; i++)
                {
                    if (i == 0) continue;
                    await _scheduledTaskRepository.DeleteAsync(cloudSyncs[i].Id);
                }
            }

            return new GetResult
            {
                CloudType = cloudType,
                IsCloudCdn = config.IsCloudCdn && isTask,
                IsCloudCdnImages = config.IsCloudCdnImages,
                IsCloudCdnFiles = config.IsCloudCdnFiles,
            };
        }
    }
}
