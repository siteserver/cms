using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Logs
{
    [Route("admin/settings/logsConfig")]
    public partial class LogsConfigController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public LogsConfigController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsConfig))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            return new GetResult
            {
                Config = config
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsConfig))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            config.IsTimeThreshold = request.IsTimeThreshold;
            if (config.IsTimeThreshold)
            {
                config.TimeThreshold = request.TimeThreshold;
            }

            config.IsLogSite = request.IsLogSite;
            config.IsLogAdmin = request.IsLogAdmin;
            config.IsLogUser = request.IsLogUser;
            config.IsLogError = request.IsLogError;

            await DataProvider.ConfigRepository.UpdateAsync(config);

            await auth.AddAdminLogAsync("修改日志设置");

            return new GetResult
            {
                Config = config
            };
        }
    }
}
