using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesUrlController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            var sites = await _siteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                Web = await _pathManager.GetSiteUrlAsync(x, false),
                Assets = await _pathManager.GetAssetsUrlAsync(x),
                Api = _pathManager.GetApiHostUrl(x, Constants.ApiPrefix)
            });

            return new GetResult
            {
                Sites = sites
            };
        }
    }
}
