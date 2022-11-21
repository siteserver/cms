using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    public partial class LayerVideoUploadController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<SubmitResult>>> Submit([FromBody] SubmitRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var vodSettings = await _vodManager.GetVodSettingsAsync();
            var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Videos);

            var result = new List<SubmitResult>();
            foreach (var filePath in request.FilePaths)
            {
                if (string.IsNullOrEmpty(filePath)) continue;

                var fileName = PathUtils.GetFileName(filePath);

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


                result.Add(new SubmitResult
                {
                    PlayUrl = playUrl,
                    VirtualUrl = virtualUrl,
                    CoverUrl = coverUrl
                });
            }

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerVideoUploadController)), new Options
            {
                IsChangeFileName = true,
                IsLibrary = false
            });

            options.IsChangeFileName = request.IsChangeFileName;
            options.IsLibrary = request.IsLibrary;
            site.Set(nameof(LayerVideoUploadController), TranslateUtils.JsonSerialize(options));

            await _siteRepository.UpdateAsync(site);

            return result;
        }
    }
}
