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
    public partial class ImageController : ControllerBase
    {
        private const string Route = "common/library/image";
        private const string RouteId = "common/library/image/{id}";
        private const string RouteList = "common/library/image/list";
        private const string RouteGroupId = "common/library/image/groups/{id}";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryImageRepository _libraryImageRepository;

        public ImageController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, ILibraryGroupRepository libraryGroupRepository, ILibraryImageRepository libraryImageRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _libraryGroupRepository = libraryGroupRepository;
            _libraryImageRepository = libraryImageRepository;
        }

        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<QueryResult>> List([FromBody]QueryRequest req)
        {
            if (!await _authManager.HasSitePermissionsAsync(req.SiteId,
                AuthTypes.SitePermissions.LibraryImage, AuthTypes.OpenPermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.Image);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                GroupName = "全部图片"
            });
            var count = await _libraryImageRepository.GetCountAsync(req.GroupId, req.Keyword);
            var items = await _libraryImageRepository.GetAllAsync(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<LibraryImage>> Create([FromQuery]CreateRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage, AuthTypes.OpenPermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var library = new LibraryImage
            {
                GroupId = request.GroupId
            };

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var extName = PathUtils.GetExtension(fileName);
            if (!_pathManager.IsImageExtensionAllowed(site, extName))
            {
                return this.Error("此图片格式已被禁止上传，请转换格式后上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);
            
            var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            await _pathManager.UploadAsync(file, filePath);

            library.Title = fileName;
            library.Url = PageUtils.Combine(virtualDirectoryPath, libraryFileName);

            library.Id = await _libraryImageRepository.InsertAsync(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public async Task<ActionResult<LibraryImage>> Update([FromRoute]int id, [FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage, AuthTypes.OpenPermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var lib = await _libraryImageRepository.GetAsync(id);
            lib.Title = request.Title;
            lib.GroupId = request.GroupId;
            await _libraryImageRepository.UpdateAsync(lib);

            return lib;
        }

        [HttpDelete, Route(RouteId)]
        public async Task<ActionResult<BoolResult>> Delete([FromRoute] int id, [FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage, AuthTypes.OpenPermissions.LibraryImage))
            {
                return Unauthorized();
            }

            await _libraryImageRepository.DeleteAsync(id);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpDelete, Route(RouteGroupId)]
        public async Task<ActionResult<BoolResult>> DeleteGroup([FromRoute] int id, [FromBody]SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.LibraryImage, AuthTypes.OpenPermissions.LibraryImage))
            {
                return Unauthorized();
            }

            await _libraryGroupRepository.DeleteAsync(LibraryType.Image, id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
