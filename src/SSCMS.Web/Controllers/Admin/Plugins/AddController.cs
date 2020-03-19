using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [Route("admin/plugins/add")]
    public partial class AddController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;

        public AddController(ISettingsManager settingsManager, IAuthManager authManager, IPluginManager pluginManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var plugins = _pluginManager.GetPlugins();
            var packageIds = Utilities.ToString(plugins.Select(x => x.PluginId));

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                PluginVersion = _settingsManager.PluginVersion,
                PackageIds = packageIds
            };
        }
    }
}
