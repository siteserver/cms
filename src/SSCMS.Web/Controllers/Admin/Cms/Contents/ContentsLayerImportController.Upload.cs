using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerImportController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var url = string.Empty;

            if (request.ImportType == "zip")
            {
                if (!FileUtils.IsFileType(FileType.Zip, PathUtils.GetExtension(fileName)))
                {
                  return this.Error(Constants.ErrorUpload);
                }
            }
            else if (request.ImportType == "excel")
            {
                if (!FileUtils.IsFileType(FileType.Xlsx, PathUtils.GetExtension(fileName)))
                {
                  return this.Error(Constants.ErrorUpload);
                }
            }
            else if (request.ImportType == "image")
            {
                if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
                {
                  return this.Error(Constants.ErrorUpload);
                }

                (_, filePath, _) = await _pathManager.UploadImageAsync(site, file);
                url = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
            }
            else if (request.ImportType == "txt")
            {
                if (!FileUtils.IsFileType(FileType.Txt, PathUtils.GetExtension(fileName)))
                {
                  return this.Error(Constants.ErrorUpload);
                }
            }
            
            await _pathManager.UploadAsync(file, filePath);

            return new UploadResult
            {
                Name = fileName,
                Url = url
            };
        }
    }
}