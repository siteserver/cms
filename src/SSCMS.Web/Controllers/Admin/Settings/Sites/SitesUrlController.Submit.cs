using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesUrlController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(request.SeparatedWebUrl) && !request.SeparatedWebUrl.EndsWith("/"))
            {
                request.SeparatedWebUrl = request.SeparatedWebUrl + "/";
            }

            if (!string.IsNullOrEmpty(request.SeparatedApiUrl) && !request.SeparatedApiUrl.EndsWith("/"))
            {
                request.SeparatedApiUrl = request.SeparatedApiUrl + "/";
            }
            if (StringUtils.EndsWithIgnoreCase(request.SeparatedApiUrl, "/api/"))
            {
                request.SeparatedApiUrl =
                    StringUtils.ReplaceEndsWithIgnoreCase(request.SeparatedApiUrl, "/api/", string.Empty);
            }
            if (!string.IsNullOrEmpty(request.SeparatedApiUrl) && !request.SeparatedApiUrl.EndsWith("/"))
            {
                request.SeparatedApiUrl = request.SeparatedApiUrl + "/";
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.IsSeparatedWeb = request.IsSeparatedWeb;
            site.SeparatedWebUrl = request.SeparatedWebUrl;

            site.IsSeparatedAssets = request.IsSeparatedAssets;
            site.SeparatedAssetsUrl = request.SeparatedAssetsUrl;
            site.AssetsDir = request.AssetsDir;

            site.IsSeparatedApi = request.IsSeparatedApi;
            site.SeparatedApiUrl = request.SeparatedApiUrl;

            await _siteRepository.UpdateAsync(site);
            await _authManager.AddSiteLogAsync(request.SiteId, "修改站点访问地址");

            var sites = await _siteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                Web = await _pathManager.GetSiteUrlAsync(x, false),
                Assets = await _pathManager.GetAssetsUrlAsync(x),
                Api = _pathManager.GetApiHostUrl(x, Constants.ApiPrefix)
            });

            return new SubmitResult
            {
                Sites = sites
            };
        }
    }
}
