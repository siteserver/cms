using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ViewController
    {
        [HttpPost, Route(RouteActionsDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(request.PluginId))
            {
                return this.Error("参数不正确");
            }

            _pluginManager.UnInstall(request.PluginId);

            await _authManager.AddAdminLogAsync("卸载插件", $"插件:{request.PluginId}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
