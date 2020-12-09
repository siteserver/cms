using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ManageController
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
