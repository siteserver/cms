using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/videoLayerUpload")]
    public partial class PagesVideoLayerUploadController : ApiController
    {
        private const string RouteUpload = "actions/upload";

        [HttpPost, Route(RouteUpload)]
        public async Task<UploadResult> Upload([FromUri]SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<UploadResult>();

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的视频文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtility.IsVideoExtensionAllowed(site, PathUtils.GetExtension(fileName)))
            {
                return Request.BadRequest<UploadResult>("视频格式不正确，请更换文件上传!");
            }

            var localDirectoryPath = await PathUtility.GetUploadDirectoryPathAsync(site, UploadType.Video);
            var localFileName = PathUtility.GetUploadFileName(fileName, true);
            var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var fileUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadResult
            {
                Name = localFileName,
                Path = filePath,
                Url = fileUrl
            };
        }
    }
}
