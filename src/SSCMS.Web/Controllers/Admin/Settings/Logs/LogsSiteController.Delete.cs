using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsSiteController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsSite))
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
