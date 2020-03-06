using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Plugins
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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var dict = await _pluginManager.GetPluginIdAndVersionDictAsync();
            var list = dict.Keys.ToList();
            var packageIds = Utilities.ToString(list);

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                PluginVersion = _settingsManager.PluginVersion,
                PackageIds = packageIds
            };
        }
    }
}
