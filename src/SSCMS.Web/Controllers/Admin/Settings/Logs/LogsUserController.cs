using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Constants.RoleTypeAdministrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsUserController : ControllerBase
    {
        private const string Route = "settings/logsUser";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;

        public LogsUserController(IAuthManager authManager, IUserRepository userRepository, IUserLogRepository userLogRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
            _userLogRepository = userLogRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<PageResult<UserLog>>> List([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Constants.AppPermissions.SettingsLogsUser))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserNameAsync(request.UserName);
            var userId = user?.Id ?? 0;

            var count = await _userLogRepository.GetCountAsync(userId, request.Keyword, request.DateFrom, request.DateTo);
            var logs = await _userLogRepository.GetAllAsync(userId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            foreach (var log in logs)
            {
                var userName = await _userRepository.GetDisplayAsync(log.UserId);
                log.Set("userName", userName);
            }

            return new PageResult<UserLog>
            {
                Items = logs,
                Count = count
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(Constants.AppPermissions.SettingsLogsUser))
            {
                return Unauthorized();
            }

            await _userLogRepository.DeleteAllAsync();

            await _authManager.AddAdminLogAsync("清空用户日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
