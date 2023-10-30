using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUploadImage)]
        public async Task<ActionResult<StringResult>> UploadImage([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("图片文件不存在！");
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

            return new StringResult
            {
                Value = imageUrl
            };
        }
    }
}
