using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class VideoController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<MaterialVideo>> Create([FromQuery] CreateRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.MaterialVideo))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var fileType = PathUtils.GetExtension(fileName);
            if (!_pathManager.IsUploadExtensionAllowed(UploadType.Video, site, fileType))
            {
                return this.Error("文件只能是视频格式，请选择有效的文件上传!");
            }

            var materialVideoName = PathUtils.GetMaterialFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Video);

            var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, materialVideoName);

            await _pathManager.UploadAsync(file, filePath);

            var video = new MaterialVideo
            {
                GroupId = request.GroupId,
                Title = PathUtils.RemoveExtension(fileName),
                FileType = fileType.ToUpper().Replace(".", string.Empty),
                Url = PageUtils.Combine(virtualDirectoryPath, materialVideoName)
            };

            await _materialVideoRepository.InsertAsync(video);

            return video;
        }
    }
}