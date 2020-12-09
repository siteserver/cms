using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsConfigController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.IsTimeThreshold = request.IsTimeThreshold;
            if (config.IsTimeThreshold)
            {
                config.TimeThreshold = request.TimeThreshold;
            }

            config.IsLogSite = request.IsLogSite;
            config.IsLogAdmin = request.IsLogAdmin;
            config.IsLogUser = request.IsLogUser;
            config.IsLogError = request.IsLogError;

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改日志设置");

            return new GetResult
            {
                Config = config
            };
        }
    }
}
