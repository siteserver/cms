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

namespace SSCMS.Web.Controllers.Admin.Common.Library
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class EditorController : ControllerBase
    {
        private const string Route = "common/library/editor";
        private const string RouteId = "common/library/editor/{id}";
        private const string RouteUpload = "common/library/editor/actions/upload";

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

        [HttpGet, Route(RouteId)]
        public async Task<ActionResult<LibraryCard>> Get([FromBody]GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard, AuthTypes.OpenPermissions.LibraryCard))
            {
                return Unauthorized();
            }

            return await _libraryTextRepository.GetAsync(request.Id);
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<LibraryCard>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard, AuthTypes.OpenPermissions.LibraryCard))
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

            var library = new LibraryCard
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
        public async Task<ActionResult<LibraryCard>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard, AuthTypes.OpenPermissions.LibraryCard))
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
        public async Task<ActionResult<StringResult>> Upload([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard, AuthTypes.OpenPermissions.LibraryCard))
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
