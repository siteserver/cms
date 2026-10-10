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
            else
            {
                return this.Error(Constants.ErrorUpload);
            }

            // 规范化后校验：目标文件必须位于对应类型资源目录内（防止 css/../.. 之类的目录穿越）
            if (!AssetPathUtils.IsSafeFileName(fileName) || !AssetPathUtils.IsSafeRelativePath(directories))
            {
                return this.Error("资源文件路径错误！");
            }
            var rootPath = await _pathManager.GetSitePathAsync(site, GetAssetsDir(site, request.FileType));
            var directoryPath = await _pathManager.GetSitePathAsync(site, directories);
            var filePath = PathUtils.Combine(directoryPath, fileName);
            if (!AssetPathUtils.IsUnder(rootPath, filePath))
            {
                return this.Error("资源文件路径错误！");
            }

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
