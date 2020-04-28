using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesUrlController : ControllerBase
    {
        private const string Route = "settings/sitesUrl";
        private const string RouteWeb = "settings/sitesUrl/actions/web";
        private const string RouteApi = "settings/sitesUrl/actions/api";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IConfigRepository _configRepository;

        public SitesUrlController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            var sites = await _siteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                SiteUrl = await _pathManager.GetSiteUrlAsync(x, false),
                AssetsUrl = await _pathManager.GetAssetsUrlAsync(x)
            });

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Sites = sites,
                IsSeparatedApi = config.IsSeparatedApi,
                SeparatedApiUrl = config.SeparatedApiUrl
            };
        }

        [HttpPut, Route(RouteWeb)]
        public async Task<ActionResult<EditWebResult>> EditWeb([FromBody]EditWebRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesUrl))
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
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesUrl))
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
