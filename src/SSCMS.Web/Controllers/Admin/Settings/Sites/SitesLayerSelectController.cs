using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesLayerSelectController : ControllerBase
    {
        public const string Route = "settings/sitesLayerSelect";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;

        public SitesLayerSelectController(IAuthManager authManager, IPathManager pathManager, IOldPluginManager pluginManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var rootSiteId = await _siteRepository.GetIdByIsRootAsync();

            var sites = await _siteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                SiteUrl = await _pathManager.GetSiteUrlAsync(x, false)
            });

            var tableNames = await _siteRepository.GetSiteTableNamesAsync(_pluginManager);

            return new GetResult
            {
                Sites = sites,
                RootSiteId = rootSiteId,
                TableNames = tableNames
            };
        }
    }
}
