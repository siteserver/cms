using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/editorLayerText")]
    public partial class PagesEditorLayerTextController : ApiController
    {
        private const string RouteId = "{id}";
        private const string RouteList = "list";

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

        [HttpGet, Route(RouteId)]
        public async Task<LibraryText> Get([FromUri]int id)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(auth.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<LibraryText>();
            }

            return await DataProvider.LibraryTextRepository.GetAsync(id);
        }
    }
}
