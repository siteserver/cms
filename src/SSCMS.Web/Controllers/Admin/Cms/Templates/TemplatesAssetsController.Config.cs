using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsController
    {
        [HttpPost, Route(RouteConfig)]
        public async Task<ActionResult<GetResult>> Config([FromBody] ConfigRequest request)
        {
            if (request.FileType == "html")
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesIncludes))
                {
                    return Unauthorized();
                }
            }
            else
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesAssets))
                {
                    return Unauthorized();
                }
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            if (request.FileType == "html")
            {
                site.TemplatesAssetsIncludeDir = request.IncludeDir.Trim('/');
            }
            else
            {
                site.TemplatesAssetsCssDir = request.CssDir.Trim('/');
                site.TemplatesAssetsJsDir = request.JsDir.Trim('/');
            }

            await _siteRepository.UpdateAsync(site);
            await _authManager.AddSiteLogAsync(request.SiteId, "资源文件文件夹设置");

            var directories = new List<Cascade<string>>();
            var files = new List<AssetFile>();

            if (request.FileType == "html")
            {
                await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsIncludeDir, ExtInclude);
            }
            else
            {
                await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsCssDir, ExtCss);
                await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsJsDir, ExtJs);
            }

            var siteUrl = (await _pathManager.GetSiteUrlAsync(site, string.Empty, true)).TrimEnd('/');

            return new GetResult
            {
                Directories = directories,
                Files = files,
                SiteUrl = siteUrl,
                IncludeDir = site.TemplatesAssetsIncludeDir,
                CssDir = site.TemplatesAssetsCssDir,
                JsDir = site.TemplatesAssetsJsDir
            };
        }
    }
}
