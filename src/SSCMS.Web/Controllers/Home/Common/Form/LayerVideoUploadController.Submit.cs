using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Common.Form
{
    public partial class LayerVideoUploadController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<SubmitResult>>> Submit([FromBody] SubmitRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Videos);

            var result = new List<SubmitResult>();
            foreach (var path in request.FilePaths)
            {
                if (string.IsNullOrEmpty(path)) continue;

                var filePath = _pathManager.ParsePath(path);
                var fileName = PathUtils.GetFileName(filePath);

                var virtualUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
                var playUrl = await _pathManager.ParseSiteUrlAsync(site, virtualUrl, true);
                var coverUrl = string.Empty;

                if (isAutoStorage)
                {
                    var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                    if (success)
                    {
                        virtualUrl = playUrl = url;
                    }
                }

                result.Add(new SubmitResult
                {
                    PlayUrl = playUrl,
                    VirtualUrl = virtualUrl,
                    CoverUrl = coverUrl
                });
            }

            var options = TranslateUtils.JsonDeserialize(site.Get<string>($"Home.{nameof(LayerVideoUploadController)}"), new Options
            {
                IsChangeFileName = true
            });

            options.IsChangeFileName = request.IsChangeFileName;
            site.Set($"Home.{nameof(LayerVideoUploadController)}", TranslateUtils.JsonSerialize(options));

            await _siteRepository.UpdateAsync(site);

            return result;
        }
    }
}
