using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteActionsUploadVideo)]
        public async Task<ActionResult<UploadVideoResult>> UploadVideo([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return new UploadVideoResult
                {
                    Error = "请选择有效的文件上传"
                };
            }

            var original = Path.GetFileName(file.FileName);
            var fileName = _pathManager.GetUploadFileName(site, original);

            if (!_pathManager.IsVideoExtensionAllowed(site, PathUtils.GetExtension(fileName)))
            {
                return new UploadVideoResult
                {
                    Error = "请选择有效的文件上传"
                };
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Video);
            var filePath = PathUtils.Combine(localDirectoryPath, fileName);

            await _pathManager.UploadAsync(file, filePath);

            var fileUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadVideoResult
            {
                State = "SUCCESS",
                Url = fileUrl,
                Title = original,
                Original = original,
                Error = null
            };
        }

        public class UploadVideoResult
        {
            public string State { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string Original { get; set; }
            public string Error { get; set; }
        }
    }
}
