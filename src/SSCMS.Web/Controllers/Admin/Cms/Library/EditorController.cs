using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class EditorController : ControllerBase
    {
        private const string Route = "cms/library/editor";
        private const string RouteUpload = "cms/library/editor/actions/upload";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ILibraryCardRepository _libraryTextRepository;

        public EditorController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ILibraryCardRepository libraryTextRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _libraryTextRepository = libraryTextRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            var library = await _libraryTextRepository.GetAsync(request.LibraryId);

            return new GetResult
            {
                Library = library
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<LibraryCard>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(request.Title))
            {
                return this.Error("请填写图文标题");
            }
            if (string.IsNullOrEmpty(request.Body))
            {
                return this.Error("请填写图文正文");
            }

            var library = new LibraryCard
            {
                Title = request.Title,
                GroupId = request.GroupId,
                ImageUrl = request.ImageUrl,
                Summary = request.Summary,
                Body = request.Body
            };

            library.Id = await _libraryTextRepository.InsertAsync(library);

            return library;
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<LibraryCard>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            var lib = await _libraryTextRepository.GetAsync(request.LibraryId);
            lib.Title = request.Title;
            lib.Body = request.Body;
            lib.ImageUrl = request.ImageUrl;
            lib.Summary = request.Summary;
            await _libraryTextRepository.UpdateAsync(lib);

            return lib;
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".svg", ".webp"))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);

            var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            await _pathManager.UploadAsync(file, filePath);

            return new StringResult
            {
                Value = PageUtils.Combine(virtualDirectoryPath, libraryFileName)
            };
        }
    }
}
