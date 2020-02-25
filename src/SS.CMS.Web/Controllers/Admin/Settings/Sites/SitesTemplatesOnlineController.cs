using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Route("admin/settings/sitesTemplatesOnline")]
    public partial class SitesTemplatesOnlineController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public SitesTemplatesOnlineController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplatesOnline))
            {
                return Unauthorized();
            }

            var siteAddPermission =
                await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd);

            return new GetResult
            {
                SiteAddPermission = siteAddPermission
            };
        }
    }
}
