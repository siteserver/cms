using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.API.Results;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/libraryEditor")]
    public class PagesLibraryEditorController : ApiController
    {
        private const string Route = "";
        private const string RouteId = "{id}";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(RouteId)]
        public LibraryTextInfo Get([FromUri]int id)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryTextInfo>();
            }

            return DataProvider.LibraryTextDao.Get(id);
        }

        [HttpPost, Route(Route)]
        public LibraryTextInfo Create([FromBody] LibraryTextInfo library)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryTextInfo>();
            }

            if (string.IsNullOrEmpty(library.Title))
            {
                return Request.BadRequest<LibraryTextInfo>("请填写图文标题");
            }
            if (string.IsNullOrEmpty(library.Content))
            {
                return Request.BadRequest<LibraryTextInfo>("请填写图文正文");
            }

            library.Content = PathUtils.SaveLibraryImage(library.Content);
            library.Id = DataProvider.LibraryTextDao.Insert(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public LibraryTextInfo Update([FromUri]int id, [FromBody] LibraryTextInfo library)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryTextInfo>();
            }

            var lib = DataProvider.LibraryTextDao.Get(id);
            lib.Title = library.Title;
            lib.Content = library.Content;
            lib.ImageUrl = library.ImageUrl;
            lib.Summary = library.Summary;
            DataProvider.LibraryTextDao.Update(lib);

            return library;
        }

        [HttpPost, Route(RouteUpload)]
        public GenericResult<string> Upload()
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<GenericResult<string>>();
            }

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<GenericResult<string>>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".svg", ".webp"))
            {
                return Request.BadRequest<GenericResult<string>>("文件只能是图片格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualPath(EUploadType.Image, libraryFileName);
            
            var directoryPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            return new GenericResult<string>
            {
                Value = PageUtils.Combine(virtualDirectoryPath, libraryFileName)
            };
        }
    }
}
