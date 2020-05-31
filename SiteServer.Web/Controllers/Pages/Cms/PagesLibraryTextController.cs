using System.IO;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.API.Results;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/libraryText")]
    public partial class PagesLibraryTextController : ApiController
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

            var groups = DataProvider.LibraryGroupDao.GetAll(LibraryType.Text);
            groups.Insert(0, new LibraryGroupInfo
            {
                Id = 0,
                GroupName = "全部图文"
            });
            var count = DataProvider.LibraryTextDao.GetCount(req.GroupId, req.Keyword);
            var items = DataProvider.LibraryTextDao.GetAll(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(Route)]
        public LibraryTextInfo Create()
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryTextInfo>();
            }

            var library = new LibraryTextInfo
            {
                GroupId = auth.GetQueryInt("groupId")
            };

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<LibraryTextInfo>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".doc") && !StringUtils.EqualsIgnoreCase(sExt, ".docx") && !StringUtils.EqualsIgnoreCase(sExt, ".wps"))
            {
                return Request.BadRequest<LibraryTextInfo>("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var libraryFileName = PathUtils.GetLibraryFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetLibraryVirtualPath(EUploadType.Image, libraryFileName);
            
            var directoryPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, libraryFileName);

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var wordContent = WordUtils.Parse(auth.SiteId, filePath, true, true, true, true, false);
            FileUtils.DeleteFileIfExists(filePath);

            library.Title = fileName;
            library.Content = wordContent;
            library.Id = DataProvider.LibraryTextDao.Insert(library);

            return library;
        }

        [HttpPut, Route(RouteId)]
        public LibraryTextInfo Update([FromUri] int id, [FromBody] LibraryTextInfo library)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryTextInfo>();
            }

            var lib = DataProvider.LibraryTextDao.Get(id);
            lib.GroupId = library.GroupId;
            DataProvider.LibraryTextDao.Update(lib);

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

            DataProvider.LibraryTextDao.Delete(id);

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
                LibraryType = LibraryType.Text,
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

            DataProvider.LibraryGroupDao.Delete(LibraryType.Text, id);

            return new DefaultResult
            {
                Value = true
            };
        }
    }
}
