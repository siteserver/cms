using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsErrorController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsError))
            {
                return Unauthorized();
            }

            await _errorLogRepository.DeleteAllAsync();

            await _authManager.AddAdminLogAsync("清空错误日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
