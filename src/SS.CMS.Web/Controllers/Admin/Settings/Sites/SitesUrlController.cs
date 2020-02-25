using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route("admin/settings/sitesUrl")]
    public partial class SitesUrlController : ControllerBase
    {
        private const string Route = "";
        private const string RouteWeb = "actions/web";
        private const string RouteApi = "actions/api";

        private readonly IAuthManager _authManager;

        public SitesUrlController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            var rootSiteId = await DataProvider.SiteRepository.GetIdByIsRootAsync();
            var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync(0);
            var sites = new List<Site>();
            foreach (var siteId in siteIdList)
            {
                sites.Add(await DataProvider.SiteRepository.GetAsync(siteId));
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(request.SeparatedWebUrl) && !request.SeparatedWebUrl.EndsWith("/"))
            {
                request.SeparatedWebUrl = request.SeparatedWebUrl + "/";
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            site.IsSeparatedWeb = request.IsSeparatedWeb;
            site.SeparatedWebUrl = request.SeparatedWebUrl;

            site.IsSeparatedAssets = request.IsSeparatedAssets;
            site.SeparatedAssetsUrl = request.SeparatedAssetsUrl;
            site.AssetsDir = request.AssetsDir;

            await DataProvider.SiteRepository.UpdateAsync(site);
            await auth.AddSiteLogAsync(request.SiteId, "修改站点访问地址");

            var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync(0);
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                sites.Add(await DataProvider.SiteRepository.GetAsync(id));
            }

            return new EditWebResult
            {
                Sites = sites
            };
        }

        [HttpPut, Route(RouteApi)]
        public async Task<ActionResult<BoolResult>> EditApi([FromBody]EditApiRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            config.IsSeparatedApi = request.IsSeparatedApi;
            config.SeparatedApiUrl = request.SeparatedApiUrl;

            await DataProvider.ConfigRepository.UpdateAsync(config);

            await auth.AddAdminLogAsync("修改API访问地址");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
