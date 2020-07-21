using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
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
    public partial class ImageController : ControllerBase
    {
        private const string Route = "cms/library/image";
        private const string RouteActionsDeleteGroup = "cms/library/image/actions/deleteGroup";
        private const string RouteActionsPull = "cms/library/image/actions/pull";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IOpenManager _openManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryImageRepository _libraryImageRepository;
        private readonly IOpenAccountRepository _openAccountRepository;

        public ImageController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IOpenManager openManager, ISiteRepository siteRepository, ILibraryGroupRepository libraryGroupRepository, ILibraryImageRepository libraryImageRepository, IOpenAccountRepository openAccountRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _openManager = openManager;
            _siteRepository = siteRepository;
            _libraryGroupRepository = libraryGroupRepository;
            _libraryImageRepository = libraryImageRepository;
            _openAccountRepository = openAccountRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery]QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var isOpen = false;
            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);
            if (account.WxConnected)
            {
                isOpen = true;
            }

            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.Image);
            var count = await _libraryImageRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _libraryImageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items,
                IsOpen = isOpen
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<LibraryImage>> Create([FromQuery]CreateRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
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

        [HttpPut, Route(Route)]
        public async Task<ActionResult<LibraryImage>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var lib = await _libraryImageRepository.GetAsync(request.Id);
            lib.Title = request.Title;
            lib.GroupId = request.GroupId;
            await _libraryImageRepository.UpdateAsync(lib);

            return lib;
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var library = await _libraryImageRepository.GetAsync(request.Id);
            var filePath = _pathManager.GetLibraryFilePath(library.Url);
            FileUtils.DeleteFileIfExists(filePath);

            await _libraryImageRepository.DeleteAsync(request.Id);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpDelete, Route(RouteActionsDeleteGroup)]
        public async Task<ActionResult<BoolResult>> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            await _libraryGroupRepository.DeleteAsync(LibraryType.Image, request.Id);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsPull)]
        public async Task<ActionResult<BoolResult>> Pull([FromBody] PullRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);
            var (success, token, errorMessage) = _openManager.GetWxAccessToken(account.WxAppId, account.WxAppSecret);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var count = await MediaApi.GetMediaCountAsync(token);
            var list = await MediaApi.GetOthersMediaListAsync(token, UploadMediaFileType.image, 0, count.image_count);

            foreach (var image in list.item)
            {
                if (await _libraryImageRepository.IsExistsAsync(image.media_id)) continue;

                await using var ms = new MemoryStream();
                await MediaApi.GetForeverMediaAsync(token, image.media_id, ms);
                ms.Seek(0, SeekOrigin.Begin);

                var extName = image.url.Substring(image.url.LastIndexOf("=", StringComparison.Ordinal) + 1);

                var libraryFileName = PathUtils.GetLibraryFileNameByExtName(extName);
                var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);

                var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
                var filePath = PathUtils.Combine(directoryPath, libraryFileName);

                await FileUtils.WriteStreamAsync(filePath, ms);

                var library = new LibraryImage
                {
                    GroupId = request.GroupId,
                    Title = image.name,
                    Url = PageUtils.Combine(virtualDirectoryPath, libraryFileName),
                    MediaId = image.media_id
                };

                await _libraryImageRepository.InsertAsync(library);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
