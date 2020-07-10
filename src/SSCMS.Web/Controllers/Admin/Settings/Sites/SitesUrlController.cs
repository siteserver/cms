using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
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

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public SitesUrlController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
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

            return new GetResult
            {
                Sites = sites
            };
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<EditWebResult>> Edit([FromBody]EditWebRequest request)
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

            var siteIdList = await _siteRepository.GetSiteIdsAsync(0);
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
    }
}
