using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesUrlController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<EditWebResult>> Edit([FromBody] EditWebRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsSitesUrl))
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
