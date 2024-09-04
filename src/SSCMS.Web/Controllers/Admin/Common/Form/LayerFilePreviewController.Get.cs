using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    public partial class LayerFilePreviewController
    {
        [HttpGet, Route(Route)]
        public ActionResult<GetResult> Get([FromQuery] GetRequest request)
        {
            var filePath = _pathManager.GetRootPath(request.FileUrl);
            if (!FileUtils.IsFileExists(filePath)) return this.Error(Constants.ErrorNotFound);

            var extName = PathUtils.GetExtension(filePath);
            var isImage = FileUtils.IsImage(extName);
            var isVideo = FileUtils.IsVideo(extName);
            var isAudio = FileUtils.IsAudio(extName);
            var isOffice = !request.Localhost && FileUtils.IsOffice(extName);
            var isPdf = FileUtils.IsPdf(extName);

            var file = new FileInfo();
            if (!isImage && !isOffice && !isPdf)
            {
                file.Name = PathUtils.GetFileName(filePath);
                file.Size = FileUtils.GetFileSizeByFilePath(filePath);
            }

            return new GetResult
            {
                IsImage = isImage,
                IsVideo = isVideo,
                IsAudio = isAudio,
                IsOffice = isOffice,
                IsPdf = isPdf,
                Url = request.FileUrl,
                File = file,
            };
        }
    }
}
