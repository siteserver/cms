using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsAdminController : ControllerBase
    {
        private const string Route = "settings/logsAdmin";

        private readonly IAuthManager _authManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ILogRepository _logRepository;

        public LogsAdminController(IAuthManager authManager, IAdministratorRepository administratorRepository, ILogRepository logRepository)
        {
            _authManager = authManager;
            _administratorRepository = administratorRepository;
            _logRepository = logRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<PageResult<Log>>> List([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsAdmin))
            {
                return Unauthorized();
            }

            var admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
            var adminId = admin?.Id ?? 0;

            var count = await _logRepository.GetAdminLogsCountAsync(adminId, request.Keyword, request.DateFrom, request.DateTo);
            var logs = await _logRepository.GetAdminLogsAsync(adminId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            foreach (var log in logs)
            {
                var adminName = await _administratorRepository.GetDisplayAsync(log.AdminId);
                log.Set("adminName", adminName);
            }

            return new PageResult<Log>
            {
                Items = logs,
                Count = count
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsAdmin))
            {
                return Unauthorized();
            }

            await _logRepository.DeleteAllAdminLogsAsync();

            await _authManager.AddAdminLogAsync("清空管理员日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
