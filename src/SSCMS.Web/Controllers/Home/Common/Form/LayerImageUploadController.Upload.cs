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
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, siteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = PathUtils.GetFileName(file.FileName);
            var (success, filePath, errorMessage) = await _pathManager.UploadImageAsync(site, file);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            return new UploadResult
            {
                Name = fileName,
                Path = filePath
            };
        }
    }
}