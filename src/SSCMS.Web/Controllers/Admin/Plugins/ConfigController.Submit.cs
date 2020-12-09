using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Plugins;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ConfigController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var config = await _pluginManager.GetConfigAsync(request.PluginId);
            config[nameof(IPlugin.Taxis)] = request.Taxis;
            config[nameof(IPlugin.AllSites)] = request.AllSites;
            config[nameof(IPlugin.SiteIds)] = request.SiteIds;
            await _pluginManager.SaveConfigAsync(request.PluginId, config);

            await _authManager.AddAdminLogAsync("修改插件配置", $"插件:{request.PluginId}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}