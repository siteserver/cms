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
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ViewController : ControllerBase
    {
        private const string Route = "plugins/view";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;

        public ViewController(ISettingsManager settingsManager, IAuthManager authManager, IPluginManager pluginManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] string pluginId)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var plugin = _pluginManager.GetPlugin(pluginId);

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.Version,
                Installed = plugin != null,
                InstalledVersion = plugin != null ? plugin.Version : string.Empty,
                Plugin = plugin
            };
        }
    }
}
