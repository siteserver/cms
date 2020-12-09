using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsAdminController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsAdmin))
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
