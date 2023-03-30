using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesIncludesController
    {
        [HttpPost, Route(RouteConfig)]
        public async Task<ActionResult<GetResult>> Config([FromBody] ConfigRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesIncludes))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            site.TemplatesAssetsIncludeDir = request.IncludeDir.Trim('/');

            await _siteRepository.UpdateAsync(site);
            await _authManager.AddSiteLogAsync(request.SiteId, "包含文件文件夹设置");

            var directories = new List<Cascade<string>>();
            var files = new List<AssetFile>();

            await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsIncludeDir, ExtInclude);

            var siteUrl = (await _pathManager.GetSiteUrlAsync(site, string.Empty, true)).TrimEnd('/');

            return new GetResult
            {
                Directories = directories,
                Files = files,
                SiteUrl = siteUrl,
                IncludeDir = site.TemplatesAssetsIncludeDir,
            };
        }
    }
}
