using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Settings.Logs
{
    [Route("admin/settings/logsAdmin")]
    public partial class LogsAdminController : ControllerBase
    {
        private const string Route = "";

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
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsAdmin))
            {
                return Unauthorized();
            }

            var admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
            var adminId = admin?.Id ?? 0;

            var count = await _logRepository.GetCountAsync(adminId, request.Keyword, request.DateFrom, request.DateTo);
            var logs = await _logRepository.GetAllAsync(adminId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

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
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsAdmin))
            {
                return Unauthorized();
            }

            await _logRepository.DeleteAllAsync();

            await _authManager.AddAdminLogAsync("清空管理员日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
