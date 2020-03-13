using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route("admin/settings/sitesUrl")]
    public partial class SitesUrlController : ControllerBase
    {
        private const string Route = "";
        private const string RouteWeb = "actions/web";
        private const string RouteApi = "actions/api";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IConfigRepository _configRepository;

        public SitesUrlController(IAuthManager authManager, ISiteRepository siteRepository, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            var rootSiteId = await _siteRepository.GetIdByIsRootAsync();
            var siteIdList = await _siteRepository.GetSiteIdListAsync(0);
            var sites = new List<Site>();
            foreach (var siteId in siteIdList)
            {
                sites.Add(await _siteRepository.GetAsync(siteId));
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Sites = sites,
                RootSiteId = rootSiteId,
                IsSeparatedApi = config.IsSeparatedApi,
                SeparatedApiUrl = config.SeparatedApiUrl
            };
        }

        [HttpPut, Route(RouteWeb)]
        public async Task<ActionResult<EditWebResult>> EditWeb([FromBody]EditWebRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(request.SeparatedWebUrl) && !request.SeparatedWebUrl.EndsWith("/"))
            {
                request.SeparatedWebUrl = request.SeparatedWebUrl + "/";
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.IsSeparatedWeb = request.IsSeparatedWeb;
            site.SeparatedWebUrl = request.SeparatedWebUrl;

            site.IsSeparatedAssets = request.IsSeparatedAssets;
            site.SeparatedAssetsUrl = request.SeparatedAssetsUrl;
            site.AssetsDir = request.AssetsDir;

            await _siteRepository.UpdateAsync(site);
            await _authManager.AddSiteLogAsync(request.SiteId, "修改站点访问地址");

            var siteIdList = await _siteRepository.GetSiteIdListAsync(0);
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                sites.Add(await _siteRepository.GetAsync(id));
            }

            return new EditWebResult
            {
                Sites = sites
            };
        }

        [HttpPut, Route(RouteApi)]
        public async Task<ActionResult<BoolResult>> EditApi([FromBody]EditApiRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.IsSeparatedApi = request.IsSeparatedApi;
            config.SeparatedApiUrl = request.SeparatedApiUrl;

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改API访问地址");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
