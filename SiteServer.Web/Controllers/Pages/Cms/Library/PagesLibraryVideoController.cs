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
    [RoutePrefix("pages/cms/library/libraryVideo")]
    public partial class PagesLibraryVideoController : ApiController
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

            var groups = await DataProvider.LibraryGroupRepository.GetAllAsync(LibraryType.Video);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                Type = LibraryType.Video,
                GroupName = "全部文件"
            });
            var count = await DataProvider.LibraryVideoRepository.GetCountAsync(req.GroupId, req.Keyword);
            var items = await DataProvider.LibraryVideoRepository.GetAllAsync(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(Route)]
        public async Task<LibraryVideo> Create([FromUri] CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryVideo>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<LibraryVideo>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var fileType = PathUtils.GetExtension(fileName);
            if (!PathUtility.IsUploadExtensionAllowed(UploadType.Video, site, fileType))
            {
                return Request.BadRequest<LibraryVideo>("文件只能是图片格式，请选择有效的文件上传!");
            }

            var libraryVideoName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Video);

            var directoryPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryVideoName);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var library = new LibraryVideo
            {
                GroupId = request.GroupId,
                Title = PathUtils.RemoveExtension(fileName),
                Type = fileType.ToUpper().Replace(".", string.Empty),
                Url = PageUtils.Combine(virtualDirectoryPath, libraryVideoName)
            };

            await DataProvider.LibraryVideoRepository.InsertAsync(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public async Task<LibraryVideo> Update([FromUri]int id, [FromBody] LibraryVideo library)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryVideo>();
            }

            var lib = await DataProvider.LibraryVideoRepository.GetAsync(id);
            lib.Title = library.Title;
            lib.GroupId = library.GroupId;
            await DataProvider.LibraryVideoRepository.UpdateAsync(lib);

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

            await DataProvider.LibraryVideoRepository.DeleteAsync(id);

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

            var library = await DataProvider.LibraryVideoRepository.GetAsync(request.LibraryId);
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
                Type = LibraryType.Video,
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

            await DataProvider.LibraryGroupRepository.DeleteAsync(LibraryType.Video, id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
