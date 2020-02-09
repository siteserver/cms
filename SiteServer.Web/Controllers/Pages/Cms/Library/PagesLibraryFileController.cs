using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Library
{
    [RoutePrefix("pages/cms/library/libraryFile")]
    public partial class PagesLibraryFileController : ApiController
    {
        private const string Route = "";
        private const string RouteId = "{id:int}";
        private const string RouteDownload = "{siteId}/{libraryId}/{fileName}";
        private const string RouteList = "list";
        private const string RouteGroups = "groups";
        private const string RouteGroupId = "groups/{id}";

        [HttpPost, Route(RouteList)]
        public async Task<QueryResult> List([FromBody]QueryRequest req)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(req.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<QueryResult>();
            }

            var groups = await DataProvider.LibraryGroupRepository.GetAllAsync(LibraryType.File);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                Type = LibraryType.File,
                GroupName = "全部文件"
            });
            var count = await DataProvider.LibraryFileRepository.GetCountAsync(req.GroupId, req.Keyword);
            var items = await DataProvider.LibraryFileRepository.GetAllAsync(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(Route)]
        public async Task<LibraryFile> Create([FromUri] CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryFile>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<LibraryFile>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var fileType = PathUtils.GetExtension(fileName);
            if (!PathUtility.IsUploadExtensionAllowed(UploadType.File, site, fileType))
            {
                return Request.BadRequest<LibraryFile>("文件只能是图片格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.File);
            
            var directoryPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var library = new LibraryFile
            {
                GroupId = request.GroupId,
                Title = PathUtils.RemoveExtension(fileName),
                Type = fileType.ToUpper().Replace(".", string.Empty),
                Url = PageUtils.Combine(virtualDirectoryPath, libraryFileName)
            };

            await DataProvider.LibraryFileRepository.InsertAsync(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public async Task<LibraryFile> Update([FromUri]int id, [FromBody] LibraryFile library)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryFile>();
            }

            var lib = await DataProvider.LibraryFileRepository.GetAsync(id);
            lib.Title = library.Title;
            lib.GroupId = library.GroupId;
            await DataProvider.LibraryFileRepository.UpdateAsync(lib);

            return library;
        }

        [HttpDelete, Route(RouteId)]
        public async Task<BoolResult> Delete([FromUri]int id)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<BoolResult>();
            }

            await DataProvider.LibraryFileRepository.DeleteAsync(id);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpGet, Route(RouteDownload)]
        public async Task Download([FromUri]DownloadRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
            {
                 Request.Unauthorized();
                 return;
            }

            var library = await DataProvider.LibraryFileRepository.GetAsync(request.LibraryId);
            var filePath = PathUtils.GetLibraryFilePath(library.Url);
            PageUtils.Download(HttpContext.Current.Response, filePath);
        }

        [HttpPost, Route(RouteGroups)]
        public async Task<LibraryGroup> CreateGroup([FromBody] GroupRequest group)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(group.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryGroup>();
            }

            var libraryGroup = new LibraryGroup
            {
                Type = LibraryType.File,
                GroupName = group.Name
            };
            libraryGroup.Id = await DataProvider.LibraryGroupRepository.InsertAsync(libraryGroup);

            return libraryGroup;
        }

        [HttpPut, Route(RouteGroupId)]
        public async Task<LibraryGroup> RenameGroup([FromUri]int id, [FromBody] GroupRequest group)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(group.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryGroup>();
            }

            var libraryGroup = await DataProvider.LibraryGroupRepository.GetAsync(id);
            libraryGroup.GroupName = group.Name;
            await DataProvider.LibraryGroupRepository.UpdateAsync(libraryGroup);

            return libraryGroup;
        }

        [HttpDelete, Route(RouteGroupId)]
        public async Task<BoolResult> DeleteGroup([FromUri]int id)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<BoolResult>();
            }

            await DataProvider.LibraryGroupRepository.DeleteAsync(LibraryType.File, id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
