using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var filePath = _pathManager.GetTemporaryFilesPath($"{request.Guid}/{fileName}");
            await _pathManager.UploadAsync(file, filePath);

            return new StringResult
            {
                Value = fileName
            };
        }
    }
}