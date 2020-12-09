using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ManageController
    {
        [HttpPost, Route(RouteActionsRestart)]
        public async Task<ActionResult<BoolResult>> Restart()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            if (_settingsManager.IsDisablePlugins)
            {
                _settingsManager.SaveSettings(_settingsManager.IsProtectData, false, _settingsManager.DatabaseType, _settingsManager.DatabaseConnectionString, _settingsManager.RedisConnectionString, _settingsManager.AdminRestrictionHost, _settingsManager.AdminRestrictionAllowList, _settingsManager.AdminRestrictionBlockList);
            }

            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
