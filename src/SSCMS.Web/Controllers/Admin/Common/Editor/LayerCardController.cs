using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerCardController : ControllerBase
    {
        private const string RouteId = "common/editor/layerCard/{id}";
        private const string RouteList = "common/editor/layerCard/list";

        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryCardRepository _libraryTextRepository;

        public LayerCardController(ILibraryGroupRepository libraryGroupRepository, ILibraryCardRepository libraryTextRepository)
        {
            _libraryGroupRepository = libraryGroupRepository;
            _libraryTextRepository = libraryTextRepository;
        }

        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<QueryResult>> List([FromBody]QueryRequest req)
        {
            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.Card);
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
        public async Task<ActionResult<LibraryCard>> Get([FromQuery]int id)
        {
            return await _libraryTextRepository.GetAsync(id);
        }
    }
}
