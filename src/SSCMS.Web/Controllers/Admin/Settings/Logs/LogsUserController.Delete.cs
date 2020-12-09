using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsUserController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsUser))
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
