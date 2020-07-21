using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils.Office;
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
    public partial class CardController : ControllerBase
    {
        private const string Route = "cms/library/card";
        private const string RouteList = "cms/library/card/list";
        private const string RouteGroups = "cms/library/card/groups";
        private const string RouteGroupId = "cms/library/card/groups/{id}";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryCardRepository _libraryTextRepository;

        public CardController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ILibraryGroupRepository libraryGroupRepository, ILibraryCardRepository libraryTextRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _libraryGroupRepository = libraryGroupRepository;
            _libraryTextRepository = libraryTextRepository;
        }

        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<QueryResult>> List([FromBody]QueryRequest req)
        {
            if (!await _authManager.HasSitePermissionsAsync(req.SiteId,
                    AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.Card);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                GroupName = "全部图文"
            });
            var count = await _libraryTextRepository.GetCountAsync(req.GroupId, req.Keyword);
            var items = await _libraryTextRepository.GetAllAsync(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<LibraryCard>> Create([FromQuery] CreateRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            var library = new LibraryCard
            {
                GroupId = request.GroupId
            };

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileTitle = PathUtils.GetFileNameWithoutExtension(file.FileName);
            var fileName = PathUtils.GetUploadFileName(file.FileName, true);
            var extendName = PathUtils.GetExtension(fileName);

            if (!FileUtils.IsWord(extendName))
            {
                return this.Error("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);
            
            var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            await _pathManager.UploadAsync(file, filePath);

            var (title, wordContent) = await WordManager.GetWordAsync(_pathManager, null, false, true, true, true, true, false, filePath, fileTitle);
            FileUtils.DeleteFileIfExists(filePath);

            library.Title = title;
            library.ImageUrl = PageUtils.Combine(virtualDirectoryPath, libraryFileName);
            library.Body = wordContent;
            library.Id = await _libraryTextRepository.InsertAsync(library);

            return library;
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody]DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            await _libraryTextRepository.DeleteAsync(request.Id);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteGroups)]
        public async Task<ActionResult<LibraryGroup>> CreateGroup([FromBody] GroupRequest group)
        {
            if (!await _authManager.HasSitePermissionsAsync(group.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            var libraryGroup = new LibraryGroup
            {
                LibraryType = LibraryType.Card,
                GroupName = group.Name
            };
            libraryGroup.Id = await _libraryGroupRepository.InsertAsync(libraryGroup);

            return libraryGroup;
        }

        [HttpPut, Route(RouteGroupId)]
        public async Task<ActionResult<LibraryGroup>> RenameGroup([FromQuery]int id, [FromBody] GroupRequest group)
        {
            if (!await _authManager.HasSitePermissionsAsync(group.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            var libraryGroup = await _libraryGroupRepository.GetAsync(id);
            libraryGroup.GroupName = group.Name;
            await _libraryGroupRepository.UpdateAsync(libraryGroup);

            return libraryGroup;
        }

        [HttpDelete, Route(RouteGroupId)]
        public async Task<ActionResult<BoolResult>> DeleteGroup([FromBody]DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard))
            {
                return Unauthorized();
            }

            await _libraryGroupRepository.DeleteAsync(LibraryType.Card, request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
