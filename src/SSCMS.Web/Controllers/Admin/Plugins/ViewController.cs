using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Packaging;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = Constants.RoleTypeAdministrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ViewController : ControllerBase
    {
        private const string Route = "plugins/view";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IOldPluginManager _pluginManager;

        public ViewController(ISettingsManager settingsManager, IAuthManager authManager, IOldPluginManager pluginManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] string pluginId)
        {
            if (!await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var plugin = _pluginManager.GetPlugin(pluginId);
            var metadata = new PackageMetadata(plugin);

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                PluginVersion = _settingsManager.SdkVersion,
                Installed = plugin != null,
                InstalledVersion = plugin != null ? plugin.Version : string.Empty,
                Plugin = metadata
            };
        }
    }
}
