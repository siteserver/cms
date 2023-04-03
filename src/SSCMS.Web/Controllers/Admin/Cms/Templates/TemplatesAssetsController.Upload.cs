using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<BoolResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var directories = string.Empty;
            var fileName = Path.GetFileName(file.FileName);
            var extName = PathUtils.GetExtension(fileName);
            if (request.FileType == ExtCss)
            {
                directories = site.TemplatesAssetsCssDir;
                if (!string.IsNullOrEmpty(request.Directories) && StringUtils.StartsWithIgnoreCase(request.Directories, site.TemplatesAssetsCssDir + '/'))
                {
                    directories = request.Directories;
                }
                if (!StringUtils.EqualsIgnoreCase(extName, "." + ExtCss))
                {
                    return this.Error(Constants.ErrorUpload);
                }
            }
            else if (request.FileType == ExtJs)
            {
                directories = site.TemplatesAssetsJsDir;
                if (!string.IsNullOrEmpty(request.Directories) && StringUtils.StartsWithIgnoreCase(request.Directories, site.TemplatesAssetsJsDir + '/'))
                {
                    directories = request.Directories;
                }
                if (!StringUtils.EqualsIgnoreCase(extName, "." + ExtJs))
                {
                    return this.Error(Constants.ErrorUpload);
                }
            }
            else if (request.FileType == ExtImages)
            {
                directories = site.TemplatesAssetsImagesDir;
                if (!string.IsNullOrEmpty(request.Directories) && StringUtils.StartsWithIgnoreCase(request.Directories, site.TemplatesAssetsImagesDir + '/'))
                {
                    directories = request.Directories;
                }
                if (!_pathManager.IsImageExtensionAllowed(site, extName))
                {
                    return this.Error(Constants.ErrorImageExtensionAllowed);
                }
                if (!_pathManager.IsImageSizeAllowed(site, file.Length))
                {
                    return this.Error(Constants.ErrorImageSizeAllowed);
                }
            }

            var directoryPath = await _pathManager.GetSitePathAsync(site, directories);
            var filePath = PathUtils.Combine(directoryPath, fileName);

            await _pathManager.UploadAsync(file, filePath);
            if (request.FileType == ExtImages)
            {
                await _pathManager.AddWaterMarkAsync(site, filePath);
            }

            return new BoolResult
            {
                Value = true,
            };
        }
    }
}
