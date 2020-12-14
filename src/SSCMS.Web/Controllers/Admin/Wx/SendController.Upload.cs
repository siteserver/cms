using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class SendController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteActionsUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxSend))
            {
                return Unauthorized();
            }
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            MaterialImage image = null;
            MaterialAudio audio = null;
            MaterialVideo video = null;

            if (request.MaterialType == MaterialType.Image)
            {
                var fileName = Path.GetFileName(file.FileName);
                var extName = PathUtils.GetExtension(fileName);
                if (!_pathManager.IsImageExtensionAllowed(site, extName))
                {
                    return this.Error(Constants.ErrorImageExtensionAllowed);
                }
                if (!_pathManager.IsImageSizeAllowed(site, file.Length))
                {
                    return this.Error(Constants.ErrorImageSizeAllowed);
                }

                var materialFileName = PathUtils.GetMaterialFileName(fileName);
                var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Image);

                var directoryPath = PathUtils.Combine(_pathManager.WebRootPath, virtualDirectoryPath);
                var filePath = PathUtils.Combine(directoryPath, materialFileName);

                await _pathManager.UploadAsync(file, filePath);
                await _pathManager.AddWaterMarkAsync(site, filePath);

                image = new MaterialImage
                {
                    GroupId = -request.SiteId,
                    Title = fileName,
                    Url = PageUtils.Combine(virtualDirectoryPath, materialFileName)
                };

                await _materialImageRepository.InsertAsync(image);
            }
            else if (request.MaterialType == MaterialType.Audio)
            {
                var fileName = Path.GetFileName(file.FileName);
                var fileType = PathUtils.GetExtension(fileName);
                if (!_pathManager.IsAudioExtensionAllowed(site, fileType))
                {
                    return this.Error(Constants.ErrorAudioExtensionAllowed);
                }
                if (!_pathManager.IsAudioSizeAllowed(site, file.Length))
                {
                    return this.Error(Constants.ErrorAudioSizeAllowed);
                }

                var materialFileName = PathUtils.GetMaterialFileName(fileName);
                var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Audio);

                var directoryPath = PathUtils.Combine(_pathManager.WebRootPath, virtualDirectoryPath);
                var filePath = PathUtils.Combine(directoryPath, materialFileName);

                await _pathManager.UploadAsync(file, filePath);

                audio = new MaterialAudio
                {
                    GroupId = -request.SiteId,
                    Title = PathUtils.RemoveExtension(fileName),
                    FileType = fileType.ToUpper().Replace(".", string.Empty),
                    Url = PageUtils.Combine(virtualDirectoryPath, materialFileName)
                };

                await _materialAudioRepository.InsertAsync(audio);
            }
            else if (request.MaterialType == MaterialType.Video)
            {
                var fileName = Path.GetFileName(file.FileName);

                var fileType = PathUtils.GetExtension(fileName);
                if (!_pathManager.IsVideoExtensionAllowed(site, fileType))
                {
                    return this.Error(Constants.ErrorVideoExtensionAllowed);
                }
                if (!_pathManager.IsVideoSizeAllowed(site, file.Length))
                {
                    return this.Error(Constants.ErrorVideoSizeAllowed);
                }

                var materialVideoName = PathUtils.GetMaterialFileName(fileName);
                var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Video);

                var directoryPath = PathUtils.Combine(_pathManager.WebRootPath, virtualDirectoryPath);
                var filePath = PathUtils.Combine(directoryPath, materialVideoName);

                await _pathManager.UploadAsync(file, filePath);

                video = new MaterialVideo
                {
                    GroupId = -request.SiteId,
                    Title = PathUtils.RemoveExtension(fileName),
                    FileType = fileType.ToUpper().Replace(".", string.Empty),
                    Url = PageUtils.Combine(virtualDirectoryPath, materialVideoName)
                };

                await _materialVideoRepository.InsertAsync(video);
            }

            return new UploadResult
            {
                MaterialType = request.MaterialType,
                Image = image,
                Audio = audio,
                Video = video
            };
        }
    }
}
