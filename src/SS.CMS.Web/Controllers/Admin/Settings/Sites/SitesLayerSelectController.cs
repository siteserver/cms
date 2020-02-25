using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route(Constants.ApiRoute)]
    public partial class SitesLayerSelectController : ControllerBase
    {
        public const string Route = "settings/sitesLayerSelect";

        private readonly IAuthManager _authManager;

        public SitesLayerSelectController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var rootSiteId = await DataProvider.SiteRepository.GetIdByIsRootAsync();

            var sites = await DataProvider.SiteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                SiteUrl = await PageUtility.GetSiteUrlAsync(x, false)
            });

            var tableNames = await DataProvider.SiteRepository.GetSiteTableNamesAsync();

            return new GetResult
            {
                Sites = sites,
                RootSiteId = rootSiteId,
                TableNames = tableNames
            };
        }
    }
}
