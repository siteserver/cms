using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsConfigController : ControllerBase
    {
        private const string Route = "settings/logsConfig";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public LogsConfigController(IAuthManager authManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsLogsConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Config = config
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsLogsConfig))
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
