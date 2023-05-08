using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ManageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }
            
            // _pluginManager.Load();

            return new GetResult
            {
                CmsVersion = _settingsManager.Version,
                AllPlugins = _pluginManager.Plugins,
                Containerized = _settingsManager.Containerized
            };
        }
    }
}
