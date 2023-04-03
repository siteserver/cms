using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> List([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var directories = new List<Cascade<string>>();
            var files = new List<AssetFile>();
            if (request.FileType == ExtCss)
            {
                await GetDirectoriesAndFilesByFileTypeAsync(directories, files, site, site.TemplatesAssetsCssDir, ExtCss);
            }
            else if (request.FileType == ExtJs)
            {
                await GetDirectoriesAndFilesByFileTypeAsync(directories, files, site, site.TemplatesAssetsJsDir, ExtJs);
            }
            else if (request.FileType == ExtImages)
            {
                await GetImagesDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsImagesDir);
            }
            var siteUrl = (await _pathManager.GetSiteUrlAsync(site, string.Empty, true)).TrimEnd('/');

            return new GetResult
            {
                Directories = directories,
                Files = files,
                SiteUrl = siteUrl,
                CssDir = site.TemplatesAssetsCssDir,
                JsDir = site.TemplatesAssetsJsDir,
                ImagesDir = site.TemplatesAssetsImagesDir
            };
        }
    }
}
