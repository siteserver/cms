using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    public partial class LayerVideoController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUploadVideo)]
        public async Task<ActionResult<UploadVideoResult>> UploadVideo([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            var result = new UploadVideoResult();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            if (file == null)
            {
                result.Success = false;
                result.ErrorMessage = Constants.ErrorUpload;
                return result;
            }

            var fileName = Path.GetFileName(file.FileName);
            if (!_pathManager.IsVideoExtensionAllowed(site, PathUtils.GetExtension(fileName)))
            {
                result.Success = false;
                result.ErrorMessage = Constants.ErrorVideoExtensionAllowed;
                return result;
            }
            if (!_pathManager.IsVideoSizeAllowed(site, file.Length))
            {
                result.Success = false;
                result.ErrorMessage = Constants.ErrorVideoSizeAllowed;
                return result;
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Video);
            var localFileName = PathUtils.GetUploadFileName(fileName, request.IsChangeFileName);
            var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

            await _pathManager.UploadAsync(file, filePath);
            var rootUrl = _pathManager.GetRootUrlByPath(filePath);

            var vodSettings = await _vodManager.GetVodSettingsAsync();
            var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Videos);

            var virtualUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
            var playUrl = await _pathManager.ParseSiteUrlAsync(site, virtualUrl, true);
            var coverUrl = string.Empty;

            if (vodSettings.IsVod)
            {
                var vodPlay = await _vodManager.UploadVodAsync(filePath);
                if (vodPlay.Success)
                {
                    virtualUrl = playUrl = vodPlay.PlayUrl;
                    coverUrl = vodPlay.CoverUrl;
                }
            }
            else if (isAutoStorage)
            {
                var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                if (success)
                {
                    virtualUrl = playUrl = url;
                }
            }

            if (request.IsLibrary)
            {
                var url = string.Empty;
                if (vodSettings.IsVod)
                {
                    url = playUrl;
                }
                else
                {
                    var materialFileName = PathUtils.GetMaterialFileName(fileName);
                    var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Image);
                    var directoryPath = _pathManager.ParsePath(virtualDirectoryPath);
                    var materialFilePath = PathUtils.Combine(directoryPath, materialFileName);
                    DirectoryUtils.CreateDirectoryIfNotExists(materialFilePath);
                    FileUtils.CopyFile(filePath, materialFilePath, true);

                    url = PageUtils.Combine(virtualDirectoryPath, materialFileName);
                }

                var video = new MaterialVideo
                {
                    Title = fileName,
                    Url = url
                };

                await _materialVideoRepository.InsertAsync(video);
            }

            result = new UploadVideoResult
            {
                Success = true,
                PlayUrl = playUrl,
                VirtualUrl = virtualUrl,
                CoverUrl = coverUrl
            };

            // var videoUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);
            // var coverUrl = string.Empty;

            // var vodSettings = await _vodManager.GetVodSettingsAsync();
            // if (vodSettings.IsVod)
            // {
            //     var vodPlay = await _vodManager.UploadVodAsync(filePath);
            //     if (vodPlay.Success)
            //     {
            //         videoUrl = vodPlay.PlayUrl;
            //         coverUrl = vodPlay.CoverUrl;
            //     }
            // }
            // else
            // {
            //     var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Videos);
            //     if (isAutoStorage)
            //     {
            //         var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
            //         if (success)
            //         {
            //             videoUrl = url;
            //         }
            //     }
            // }

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerVideoController)), new Options
            {
                IsChangeFileName = true,
                IsLibrary = false
            });

            options.IsChangeFileName = request.IsChangeFileName;
            options.IsLibrary = request.IsLibrary;
            site.Set(nameof(LayerVideoController), TranslateUtils.JsonSerialize(options));

            await _siteRepository.UpdateAsync(site);

            return result;

            // return new UploadVideoResult
            // {
            //     Name = fileName,
            //     Url = videoUrl,
            //     CoverUrl = coverUrl
            // };
        }
    }
}
