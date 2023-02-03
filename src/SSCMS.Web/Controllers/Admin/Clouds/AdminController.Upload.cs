using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class AdminController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] string type, [FromForm] IFormFile file)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            if (file == null) return this.Error(Constants.ErrorUpload);
            var extension = PathUtils.GetExtension(file.FileName);
            if (!FileUtils.IsImage(extension))
            {
                return this.Error(Constants.ErrorImageExtensionAllowed);
            }
            var fileName = $"{type}{extension}";
            var filePath = _pathManager.GetSiteFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var url = _pathManager.GetSiteFilesUrl(fileName);

            return new UploadResult
            {
                Type = type,
                Url = url
            };
        }
    }
}
