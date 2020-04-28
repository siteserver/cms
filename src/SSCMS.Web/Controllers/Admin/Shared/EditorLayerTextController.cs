using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class EditorLayerTextController : ControllerBase
    {
        private const string RouteId = "shared/editorLayerText/{id}";
        private const string RouteList = "shared/editorLayerText/list";

        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryTextRepository _libraryTextRepository;

        public EditorLayerTextController(ILibraryGroupRepository libraryGroupRepository, ILibraryTextRepository libraryTextRepository)
        {
            _libraryGroupRepository = libraryGroupRepository;
            _libraryTextRepository = libraryTextRepository;
        }

        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<QueryResult>> List([FromBody]QueryRequest req)
        {
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
            return await _libraryTextRepository.GetAsync(id);
        }
    }
}
