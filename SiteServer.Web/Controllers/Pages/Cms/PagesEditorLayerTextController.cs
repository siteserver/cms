using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/editorLayerText")]
    public partial class PagesEditorLayerTextController : ApiController
    {
        private const string RouteId = "{id}";
        private const string RouteList = "list";

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

        [HttpGet, Route(RouteId)]
        public LibraryTextInfo Get([FromUri]int id)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(auth.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryTextInfo>();
            }

            return DataProvider.LibraryTextDao.Get(id);
        }
    }
}
