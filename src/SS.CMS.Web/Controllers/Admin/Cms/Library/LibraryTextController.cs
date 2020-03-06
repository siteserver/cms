using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Core.Office;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Library
{
    [Route("admin/cms/library/libraryText")]
    public partial class LibraryTextController : ControllerBase
    {
        private const string Route = "";
        private const string RouteId = "{id}";
        private const string RouteList = "list";
        private const string RouteGroups = "groups";
        private const string RouteGroupId = "groups/{id}";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryTextRepository _libraryTextRepository;

        public LibraryTextController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ILibraryGroupRepository libraryGroupRepository, ILibraryTextRepository libraryTextRepository)
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
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(req.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Unauthorized();
            }

            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.Text);
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
        public async Task<ActionResult<LibraryText>> Create([FromQuery] CreateRequest request, [FromForm] IFormFile file)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Unauthorized();
            }

            var library = new LibraryText
            {
                GroupId = request.GroupId
            };

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".doc") && !StringUtils.EqualsIgnoreCase(sExt, ".docx") && !StringUtils.EqualsIgnoreCase(sExt, ".wps"))
            {
                return this.Error("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);
            
            var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            await _pathManager.UploadAsync(file, filePath);

            var (_, wordContent) = await WordManager.GetWordAsync(_pathManager, null, false, true, true, true, true, false, filePath);
            FileUtils.DeleteFileIfExists(filePath);

            library.Title = fileName;
            library.ImageUrl = PageUtils.Combine(virtualDirectoryPath, libraryFileName);
            library.Content = wordContent;
            library.Id = await _libraryTextRepository.InsertAsync(library);

            return library;
        }

        [HttpDelete, Route(RouteId)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody]DeleteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
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
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(group.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Unauthorized();
            }

            var libraryGroup = new LibraryGroup
            {
                LibraryType = LibraryType.Text,
                GroupName = group.Name
            };
            libraryGroup.Id = await _libraryGroupRepository.InsertAsync(libraryGroup);

            return libraryGroup;
        }

        [HttpPut, Route(RouteGroupId)]
        public async Task<ActionResult<LibraryGroup>> RenameGroup([FromQuery]int id, [FromBody] GroupRequest group)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(group.SiteId,
                    Constants.SitePermissions.Library))
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
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Unauthorized();
            }

            await _libraryGroupRepository.DeleteAsync(LibraryType.Text, request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
