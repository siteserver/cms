using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class AddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var packageIds = _pluginManager.Plugins.Select(x => x.PluginId);

            return new GetResult
            {
                Version = _settingsManager.Version,
                PackageIds = packageIds
            };
        }
    }
}
