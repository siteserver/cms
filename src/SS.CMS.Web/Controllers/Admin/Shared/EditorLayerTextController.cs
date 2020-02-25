using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Shared
{
    [Route("admin/shared/editorLayerText")]
    public partial class EditorLayerTextController : ControllerBase
    {
        private const string RouteId = "{id}";
        private const string RouteList = "list";

        private readonly IAuthManager _authManager;

        public EditorLayerTextController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<QueryResult>> List([FromBody]QueryRequest req)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

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
        public async Task<ActionResult<LibraryText>> Get([FromQuery]int id)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

            return await DataProvider.LibraryTextRepository.GetAsync(id);
        }
    }
}
