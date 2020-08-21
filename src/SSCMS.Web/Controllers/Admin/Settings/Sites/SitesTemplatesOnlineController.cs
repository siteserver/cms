using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesTemplatesOnlineController : ControllerBase
    {
        private const string Route = "settings/sitesTemplatesOnline";

        private readonly IAuthManager _authManager;

        public SitesTemplatesOnlineController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsSitesTemplatesOnline))
            {
                return Unauthorized();
            }

            var siteAddPermission =
                await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsSitesAdd);

            return new GetResult
            {
                SiteAddPermission = siteAddPermission
            };
        }
    }
}
