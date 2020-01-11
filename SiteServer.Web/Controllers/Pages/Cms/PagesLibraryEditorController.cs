using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/libraryEditor")]
    public class PagesLibraryEditorController : ApiController
    {
        private const string Route = "";
        private const string RouteId = "{id}";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(RouteId)]
        public async Task<LibraryText> Get([FromUri]int id)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryText>();
            }

            return await DataProvider.LibraryTextRepository.GetAsync(id);
        }

        [HttpPost, Route(Route)]
        public async Task<LibraryText> Create([FromBody] LibraryText library)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryText>();
            }

            if (string.IsNullOrEmpty(library.Title))
            {
                return Request.BadRequest<LibraryText>("请填写图文标题");
            }
            if (string.IsNullOrEmpty(library.Content))
            {
                return Request.BadRequest<LibraryText>("请填写图文正文");
            }

            library.Id = await DataProvider.LibraryTextRepository.InsertAsync(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public async Task<LibraryText> Update([FromUri]int id, [FromBody] LibraryText library)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryText>();
            }

            var lib = await DataProvider.LibraryTextRepository.GetAsync(id);
            lib.Title = library.Title;
            lib.Content = library.Content;
            lib.ImageUrl = library.ImageUrl;
            lib.Summary = library.Summary;
            await DataProvider.LibraryTextRepository.UpdateAsync(lib);

            return library;
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<GenericResult<string>> Upload()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
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
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);
            
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
