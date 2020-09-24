using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsAdminController
    {
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
