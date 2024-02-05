using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerWordController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] ChannelRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileUrl = PathUtils.GetUploadFileName(file.FileName, true);
            var extendName = PathUtils.GetExtension(fileUrl);

            if (!FileUtils.IsWord(extendName))
            {
                return this.Error("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileUrl);
            await _pathManager.UploadAsync(file, filePath);

            return new UploadResult
            {
                FileName = file.FileName,
                FileUrl = fileUrl,
            };
        }
    }
}