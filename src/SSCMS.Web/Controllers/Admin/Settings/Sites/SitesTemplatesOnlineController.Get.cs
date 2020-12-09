using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesOnlineController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTemplatesOnline))
            {
                return Unauthorized();
            }

            var siteAddPermission =
                await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd);

            return new GetResult
            {
                SiteAddPermission = siteAddPermission
            };
        }
    }
}
