using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Plugins;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    [Route("admin/plugins/view")]
    public partial class ViewController : ControllerBase
    {
        private const string Route = "{pluginId}";

        private readonly IAuthManager _authManager;

        public ViewController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get(string pluginId)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var plugin = await PluginManager.GetPluginAsync(pluginId);

            return new GetResult
            {
                IsNightly = WebConfigUtils.IsNightlyUpdate,
                PluginVersion = SystemManager.PluginVersion,
                Installed = plugin != null,
                InstalledVersion = plugin != null ? plugin.Version : string.Empty,
                Package = plugin
            };
        }
    }
}
