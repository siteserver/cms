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
    public partial class ActionsController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteActionsUploadImage)]
        public async Task<ActionResult<UploadImageResult>> UploadImage([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return new UploadImageResult
                {
                    Error = Constants.ErrorUpload
                };
            }

            var original = Path.GetFileName(file.FileName);
            var (success, filePath, errorMessage) = await _pathManager.UploadImageAsync(site, file);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var imageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);
            var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Images);
            if (isAutoStorage)
            {
                (success, var url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                if (success)
                {
                    imageUrl = url;
                }
            }

            return new UploadImageResult
            {
                State = "SUCCESS",
                Url = imageUrl,
                Title = original,
                Original = original,
                Error = null
            };
        }

        public class UploadImageResult
        {
            public string State { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string Original { get; set; }
            public string Error { get; set; }
        }
    }
}
