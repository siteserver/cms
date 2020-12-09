using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsErrorController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsError))
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
