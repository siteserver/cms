using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsSiteController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            await _siteLogRepository.DeleteAllAsync();

            await _authManager.AddAdminLogAsync("清空站点日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
