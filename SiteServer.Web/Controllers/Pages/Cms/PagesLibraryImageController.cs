using System.IO;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.API.Results;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/libraryImage")]
    public partial class PagesLibraryImageController : ApiController
    {
        private const string Route = "";
        private const string RouteId = "{id}";
        private const string RouteList = "list";
        private const string RouteGroups = "groups";
        private const string RouteGroupId = "groups/{id}";

        [HttpPost, Route(RouteList)]
        public QueryResult List([FromBody]QueryRequest req)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(req.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<QueryResult>();
            }

            var groups = DataProvider.LibraryGroupDao.GetAll(LibraryType.Image);
            groups.Insert(0, new LibraryGroupInfo
            {
                Id = 0,
                GroupName = "全部图片"
            });
            var count = DataProvider.LibraryImageDao.GetCount(req.GroupId, req.Keyword);
            var items = DataProvider.LibraryImageDao.GetAll(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(Route)]
        public LibraryImageInfo Create()
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryImageInfo>();
            }

            var library = new LibraryImageInfo
            {
                GroupId = auth.GetQueryInt("groupId")
            };

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<LibraryImageInfo>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".svg", ".webp"))
            {
                return Request.BadRequest<LibraryImageInfo>("文件只能是图片格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualPath = PathUtils.GetLibraryVirtualPath(EUploadType.Image, libraryFileName);
            
            var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualPath);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            library.Title = fileName;
            library.Url = virtualPath;

            library.Id = DataProvider.LibraryImageDao.Insert(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public LibraryImageInfo Update([FromUri]int id, [FromBody] LibraryImageInfo library)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryImageInfo>();
            }

            var lib = DataProvider.LibraryImageDao.Get(id);
            lib.Title = library.Title;
            lib.GroupId = library.GroupId;
            DataProvider.LibraryImageDao.Update(lib);

            return library;
        }

        [HttpDelete, Route(RouteId)]
        public DefaultResult Delete([FromUri]int id)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<DefaultResult>();
            }

            var lib = DataProvider.LibraryImageDao.Get(id);
            var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, lib.Url);
            FileUtils.DeleteFileIfExists(filePath);

            DataProvider.LibraryImageDao.Delete(id);

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteGroups)]
        public LibraryGroupInfo CreateGroup([FromBody] GroupRequest group)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(group.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryGroupInfo>();
            }

            var libraryGroup = new LibraryGroupInfo
            {
                LibraryType = LibraryType.Image,
                GroupName = group.Name
            };
            libraryGroup.Id = DataProvider.LibraryGroupDao.Insert(libraryGroup);

            return libraryGroup;
        }

        [HttpPut, Route(RouteGroupId)]
        public LibraryGroupInfo RenameGroup([FromUri]int id, [FromBody] GroupRequest group)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(group.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryGroupInfo>();
            }

            var libraryGroup = DataProvider.LibraryGroupDao.Get(id);
            libraryGroup.GroupName = group.Name;
            DataProvider.LibraryGroupDao.Update(libraryGroup);

            return libraryGroup;
        }

        [HttpDelete, Route(RouteGroupId)]
        public DefaultResult DeleteGroup([FromUri]int id)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<DefaultResult>();
            }

            DataProvider.LibraryGroupDao.Delete(LibraryType.Image, id);

            return new DefaultResult
            {
                Value = true
            };
        }
    }
}
