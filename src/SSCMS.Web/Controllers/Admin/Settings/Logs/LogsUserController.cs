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
    public partial class LogsUserController : ControllerBase
    {
        private const string Route = "settings/logsUser";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public LogsUserController(IAuthManager authManager, IUserRepository userRepository, ILogRepository logRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<PageResult<Log>>> List([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsUser))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserNameAsync(request.UserName);
            var userId = user?.Id ?? 0;

            var count = await _logRepository.GetUserLogsCountAsync(userId, request.Keyword, request.DateFrom, request.DateTo);
            var logs = await _logRepository.GetUserLogsAsync(userId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            foreach (var log in logs)
            {
                var userName = await _userRepository.GetDisplayAsync(log.UserId);
                log.Set("userName", userName);
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
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsUser))
            {
                return Unauthorized();
            }

            await _logRepository.DeleteAllUserLogsAsync();

            await _authManager.AddAdminLogAsync("清空用户日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
