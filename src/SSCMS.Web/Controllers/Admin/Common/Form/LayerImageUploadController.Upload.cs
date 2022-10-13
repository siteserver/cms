using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    public partial class LayerImageUploadController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] int siteId, [FromQuery] int userId, [FromForm] IFormFile file)
        {
            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = PathUtils.GetFileName(file.FileName);
            var extName = PathUtils.GetExtension(fileName);
            var filePath = string.Empty;

            if (siteId > 0)
            {
                var site = await _siteRepository.GetAsync(siteId);

                (var success, filePath, var errorMessage) = await _pathManager.UploadImageAsync(site, file);
                if (!success)
                {
                    return this.Error(errorMessage);
                }
            }
            else if (userId > 0)
            {
                filePath = _pathManager.GetUserUploadPath(userId, fileName);
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