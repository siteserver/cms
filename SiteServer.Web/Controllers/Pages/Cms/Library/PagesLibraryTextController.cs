using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Library
{

    [RoutePrefix("pages/cms/library/libraryText")]
    public partial class PagesLibraryTextController : ApiController
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

            var groups = await DataProvider.LibraryGroupRepository.GetAllAsync(LibraryType.Text);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                GroupName = "全部图文"
            });
            var count = await DataProvider.LibraryTextRepository.GetCountAsync(req.GroupId, req.Keyword);
            var items = await DataProvider.LibraryTextRepository.GetAllAsync(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(Route)]
        public async Task<LibraryText> Create()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryText>();
            }

            var library = new LibraryText
            {
                GroupId = auth.GetQueryInt("groupId")
            };

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<LibraryText>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".doc") && !StringUtils.EqualsIgnoreCase(sExt, ".docx") && !StringUtils.EqualsIgnoreCase(sExt, ".wps"))
            {
                return Request.BadRequest<LibraryText>("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);
            
            var directoryPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var (_, wordContent) = await WordManager.GetWordAsync(null, false, true, true, true, true, false, filePath);
            FileUtils.DeleteFileIfExists(filePath);

            library.Title = fileName;
            library.ImageUrl = PageUtils.Combine(virtualDirectoryPath, libraryFileName);
            library.Content = wordContent;
            library.Id = await DataProvider.LibraryTextRepository.InsertAsync(library);

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

            await DataProvider.LibraryTextRepository.DeleteAsync(id);

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
                Type = LibraryType.Text,
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

            await DataProvider.LibraryGroupRepository.DeleteAsync(LibraryType.Text, id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
