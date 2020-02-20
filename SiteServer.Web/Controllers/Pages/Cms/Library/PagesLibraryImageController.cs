using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Library
{

    [RoutePrefix("pages/cms/library/libraryImage")]
    public partial class PagesLibraryImageController : ApiController
    {
        private const string Route = "";
        private const string RouteId = "{id}";
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

            var groups = await DataProvider.LibraryGroupRepository.GetAllAsync(LibraryType.Image);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                GroupName = "全部图片"
            });
            var count = await DataProvider.LibraryImageRepository.GetCountAsync(req.GroupId, req.Keyword);
            var items = await DataProvider.LibraryImageRepository.GetAllAsync(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(Route)]
        public async Task<LibraryImage> Create()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryImage>();
            }

            var library = new LibraryImage
            {
                GroupId = auth.GetQueryInt("groupId")
            };

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<LibraryImage>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".svg", ".webp"))
            {
                return Request.BadRequest<LibraryImage>("文件只能是图片格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);
            
            var directoryPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            library.Title = fileName;
            library.Url = PageUtils.Combine(virtualDirectoryPath, libraryFileName);

            library.Id = await DataProvider.LibraryImageRepository.InsertAsync(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public async Task<LibraryImage> Update([FromUri]int id, [FromBody] LibraryImage library)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryImage>();
            }

            var lib = await DataProvider.LibraryImageRepository.GetAsync(id);
            lib.Title = library.Title;
            lib.GroupId = library.GroupId;
            await DataProvider.LibraryImageRepository.UpdateAsync(lib);

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

            await DataProvider.LibraryImageRepository.DeleteAsync(id);

            return new BoolResult
            {
                Value = true
            };
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
                Type = LibraryType.Image,
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

            await DataProvider.LibraryGroupRepository.DeleteAsync(LibraryType.Image, id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
