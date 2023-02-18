using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    public partial class LayerVideoController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUploadVideo)]
        public async Task<ActionResult<UploadResult>> UploadVideo([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!_pathManager.IsVideoExtensionAllowed(site, PathUtils.GetExtension(fileName)))
            {
                return this.Error(Constants.ErrorVideoExtensionAllowed);
            }
            if (!_pathManager.IsVideoSizeAllowed(site, file.Length))
            {
                return this.Error(Constants.ErrorVideoSizeAllowed);
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Video);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, fileName));

            await _pathManager.UploadAsync(file, filePath);

            var videoUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);
            var coverUrl = string.Empty;

            var vodSettings = await _vodManager.GetVodSettingsAsync();
            if (vodSettings.IsVod)
            {
                var vodPlay = await _vodManager.UploadVodAsync(filePath);
                if (vodPlay.Success)
                {
                    videoUrl = vodPlay.PlayUrl;
                    coverUrl = vodPlay.CoverUrl;
                }
            }
            else
            {
                var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Videos);
                if (isAutoStorage)
                {
                    var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                    if (success)
                    {
                        videoUrl = url;
                    }
                }
            }

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = videoUrl,
                CoverUrl = coverUrl
            };
        }
    }
}
