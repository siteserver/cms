using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class LayerVideoUploadController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromQuery] SubmitRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            if (file == null)
            {
                return new SubmitResult
                {
                    Success = false,
                    ErrorMessage = Constants.ErrorUpload
                };
            }

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

            video.Id = await _materialVideoRepository.InsertAsync(video);

            return new SubmitResult
            {
                Success = true,
                Video = video,
            };
        }
    }
}