using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesUrlController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            var sites = await _siteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                SiteUrl = await _pathManager.GetSiteUrlAsync(x, false),
                AssetsUrl = await _pathManager.GetAssetsUrlAsync(x)
            });

            return new GetResult
            {
                Sites = sites
            };
        }
    }
}
