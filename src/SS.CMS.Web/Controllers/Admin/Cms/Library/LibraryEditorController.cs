using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Library
{
    
    [Route("admin/cms/library/libraryEditor")]
    public partial class LibraryEditorController : ControllerBase
    {
        private const string Route = "";
        private const string RouteId = "{id}";
        private const string RouteUpload = "actions/upload";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ILibraryTextRepository _libraryTextRepository;

        public LibraryEditorController(ISettingsManager settingsManager, IAuthManager authManager, ILibraryTextRepository libraryTextRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _libraryTextRepository = libraryTextRepository;
        }

        [HttpGet, Route(RouteId)]
        public async Task<ActionResult<LibraryText>> Get([FromBody]GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Unauthorized();
            }

            return await _libraryTextRepository.GetAsync(request.Id);
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<LibraryText>> Create([FromBody] CreateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(request.Title))
            {
                return this.Error("请填写图文标题");
            }
            if (string.IsNullOrEmpty(request.Content))
            {
                return this.Error("请填写图文正文");
            }

            var library = new LibraryText
            {
                Title = request.Title,
                GroupId = request.GroupId,
                ImageUrl = request.ImageUrl,
                Summary = request.Summary,
                Content = request.Content
            };

            library.Id = await _libraryTextRepository.InsertAsync(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public async Task<ActionResult<LibraryText>> Update([FromBody] UpdateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Unauthorized();
            }

            var lib = await _libraryTextRepository.GetAsync(request.Id);
            lib.Title = request.Title;
            lib.Content = request.Content;
            lib.ImageUrl = request.ImageUrl;
            lib.Summary = request.Summary;
            await _libraryTextRepository.UpdateAsync(lib);

            return lib;
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromBody]UploadRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Unauthorized();
            }

            if (request.File == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(request.File.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".svg", ".webp"))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);

            var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            request.File.CopyTo(new FileStream(filePath, FileMode.Create));

            return new StringResult
            {
                Value = PageUtils.Combine(virtualDirectoryPath, libraryFileName)
            };
        }
    }
}
