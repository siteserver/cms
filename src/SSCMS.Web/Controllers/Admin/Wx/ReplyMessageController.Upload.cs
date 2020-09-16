using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyMessageController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteActionsUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.WxReplyAuto, Types.SitePermissions.WxReplyBeAdded))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
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
                    return this.Error("此图片格式已被禁止上传，请转换格式后上传!");
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
                if (!_pathManager.IsUploadExtensionAllowed(UploadType.Audio, site, fileType))
                {
                    return this.Error("文件只能是音频格式，请选择有效的文件上传!");
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
                if (!_pathManager.IsUploadExtensionAllowed(UploadType.Video, site, fileType))
                {
                    return this.Error("文件只能是视频格式，请选择有效的文件上传!");
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
