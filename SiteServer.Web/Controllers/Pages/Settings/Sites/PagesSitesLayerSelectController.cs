using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    [RoutePrefix("pages/settings/sitesLayerSelect")]
    public partial class PagesSitesLayerSelectController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Request.Unauthorized<GetResult>();
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
