using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Common.Form
{
    public partial class LayerImageUploadController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] int siteId, [FromForm] IFormFile file)
        {
            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = string.Empty;
            var filePath = string.Empty;

            if (siteId > 0)
            {
                var siteIds = await _authManager.GetSiteIdsAsync();
                if (!ListUtils.Contains(siteIds, siteId)) return Unauthorized();

                var site = await _siteRepository.GetAsync(siteId);

                fileName = PathUtils.GetFileName(file.FileName);
                (var success, filePath, var errorMessage) = await _pathManager.UploadImageAsync(site, file);
                if (!success)
                {
                    return this.Error(errorMessage);
                }
            }
            else
            {
                fileName = _pathManager.GetUploadFileName(file.FileName);
                filePath = _pathManager.GetUserUploadPath(_authManager.UserId, fileName);
                if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
                {
                    return this.Error(Constants.ErrorImageExtensionAllowed);
                }

                await _pathManager.UploadAsync(file, filePath);
            }

            return new UploadResult
            {
                Name = fileName,
                Path = filePath
            };
        }
    }
}