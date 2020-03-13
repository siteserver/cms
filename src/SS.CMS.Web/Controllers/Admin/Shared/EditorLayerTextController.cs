using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Shared
{
    [Route("admin/shared/editorLayerText")]
    public partial class EditorLayerTextController : ControllerBase
    {
        private const string RouteId = "{id}";
        private const string RouteList = "list";

        private readonly IAuthManager _authManager;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryTextRepository _libraryTextRepository;

        public EditorLayerTextController(IAuthManager authManager, ILibraryGroupRepository libraryGroupRepository, ILibraryTextRepository libraryTextRepository)
        {
            _authManager = authManager;
            _libraryGroupRepository = libraryGroupRepository;
            _libraryTextRepository = libraryTextRepository;
        }

        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<QueryResult>> List([FromBody]QueryRequest req)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

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

        [HttpGet, Route(RouteId)]
        public async Task<ActionResult<LibraryText>> Get([FromQuery]int id)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            return await _libraryTextRepository.GetAsync(id);
        }
    }
}
