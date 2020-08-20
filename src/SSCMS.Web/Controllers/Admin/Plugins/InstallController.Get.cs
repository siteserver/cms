using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class InstallController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.Version
            };
        }
    }
}
