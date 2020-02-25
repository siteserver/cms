using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Logs
{
    [Route("admin/settings/logsUser")]
    public partial class LogsUserController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public LogsUserController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<PageResult<UserLog>>> List([FromBody] SearchRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsUser))
            {
                return Unauthorized();
            }

            var user = await DataProvider.UserRepository.GetByUserNameAsync(request.UserName);
            var userId = user?.Id ?? 0;

            var count = await DataProvider.UserLogRepository.GetCountAsync(userId, request.Keyword, request.DateFrom, request.DateTo);
            var logs = await DataProvider.UserLogRepository.GetAllAsync(userId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            foreach (var log in logs)
            {
                var userName = await DataProvider.UserRepository.GetDisplayAsync(log.UserId);
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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsUser))
            {
                return Unauthorized();
            }

            await DataProvider.UserLogRepository.DeleteAllAsync();

            await auth.AddAdminLogAsync("清空用户日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
