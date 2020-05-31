using System.IO;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/libraryLayerVideo")]
    public partial class PagesLibraryLayerVideoController : ApiController
    {
        private const string RouteUploadVideo = "actions/uploadVideo";
        private const string RouteUploadImage = "actions/uploadImage";

        [HttpPost, Route(RouteUploadVideo)]
        public UploadResult UploadVideo([FromUri]int siteId)
        {
            var req = new AuthenticatedRequest();
            if (!req.IsAdminLoggin ||
                !req.AdminPermissionsImpl.HasSitePermissions(siteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = SiteManager.GetSiteInfo(siteId);

            var fileName = req.HttpRequest["fileName"];
            var fileCount = req.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传");
            }

            var file = req.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".mp4", ".flv", ".f4v", ".webm", ".m4v", ".mov", ".3gp", ".3g2"))
            {
                return Request.BadRequest<UploadResult>("文件只能是主流视频格式，请选择有效的文件上传!");
            }

            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, EUploadType.Video);
            var filePath = PathUtils.Combine(localDirectoryPath, PathUtility.GetUploadFileName(site, fileName));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var imageUrl = PageUtility.GetSiteUrlByPhysicalPath(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = imageUrl
            };
        }

        [HttpPost, Route(RouteUploadImage)]
        public UploadResult UploadImage([FromUri]int siteId)
        {
            var req = new AuthenticatedRequest();
            if (!req.IsAdminLoggin ||
                !req.AdminPermissionsImpl.HasSitePermissions(siteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = SiteManager.GetSiteInfo(siteId);

            var fileName = req.HttpRequest["fileName"];
            var fileCount = req.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传");
            }

            var file = req.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".webp"))
            {
                return Request.BadRequest<UploadResult>("文件只能是 Image 格式，请选择有效的文件上传!");
            }

            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, EUploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, PathUtility.GetUploadFileName(site, fileName));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var imageUrl = PageUtility.GetSiteUrlByPhysicalPath(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = imageUrl
            };
        }
    }
}
