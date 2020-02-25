using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Logs
{
    [Route("admin/settings/logsAdmin")]
    public partial class LogsAdminController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public LogsAdminController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<PageResult<Log>>> List([FromBody] SearchRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsAdmin))
            {
                return Unauthorized();
            }

            var admin = await DataProvider.AdministratorRepository.GetByUserNameAsync(request.UserName);
            var adminId = admin?.Id ?? 0;

            var count = await DataProvider.LogRepository.GetCountAsync(adminId, request.Keyword, request.DateFrom, request.DateTo);
            var logs = await DataProvider.LogRepository.GetAllAsync(adminId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            foreach (var log in logs)
            {
                var adminName = await DataProvider.AdministratorRepository.GetDisplayAsync(log.AdminId);
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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsAdmin))
            {
                return Unauthorized();
            }

            await DataProvider.LogRepository.DeleteAllAsync();

            await auth.AddAdminLogAsync("清空管理员日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
