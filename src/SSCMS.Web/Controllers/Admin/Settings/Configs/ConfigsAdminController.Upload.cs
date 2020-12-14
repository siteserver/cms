using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsAdminController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromForm] IFormFile file)
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
            var fileName = $"logo{extension}";
            var filePath = _pathManager.GetSiteFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var adminLogoUrl = _pathManager.GetSiteFilesUrl(fileName);

            return new StringResult
            {
                Value = adminLogoUrl
            };
        }
    }
}
