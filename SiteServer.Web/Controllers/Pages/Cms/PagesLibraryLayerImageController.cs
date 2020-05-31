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
    [RoutePrefix("pages/cms/libraryLayerImage")]
    public partial class PagesLibraryLayerImageController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpPost, Route(RouteUpload)]
        public UploadResult Upload([FromUri]int siteId)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(siteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = SiteManager.GetSiteInfo(siteId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".webp"))
            {
                return Request.BadRequest<UploadResult>("文件只能是 Image 格式，请选择有效的文件上传!");
            }

            var virtualUrl = PathUtils.GetLibraryVirtualPath(EUploadType.Image, PathUtility.GetUploadFileName(site, fileName));
            var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualUrl);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = virtualUrl
            };
        }
    }
}
