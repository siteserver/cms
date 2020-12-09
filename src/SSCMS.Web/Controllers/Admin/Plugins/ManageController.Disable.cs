using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Plugins;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ManageController
    {
        [HttpPost, Route(RouteActionsDisable)]
        public async Task<ActionResult<BoolResult>> Disable([FromBody] DisableRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var config = await _pluginManager.GetConfigAsync(request.PluginId);
            config[nameof(IPlugin.Disabled)] = request.Disabled;
            await _pluginManager.SaveConfigAsync(request.PluginId, config);

            await _authManager.AddAdminLogAsync(request.Disabled ? "禁用插件" : "启用插件", $"插件:{request.PluginId}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
