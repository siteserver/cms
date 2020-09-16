using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerArticleController : ControllerBase
    {
        private const string RouteId = "common/editor/layerArticle/{id}";
        private const string RouteList = "common/editor/layerArticle/list";

        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialArticleRepository _materialArticleRepository;

        public LayerArticleController(IMaterialGroupRepository materialGroupRepository, IMaterialArticleRepository materialArticleRepository)
        {
            _materialGroupRepository = materialGroupRepository;
            _materialArticleRepository = materialArticleRepository;
        }

        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<QueryResult>> List([FromBody]QueryRequest req)
        {
            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Article);
            var count = await _materialArticleRepository.GetCountAsync(req.GroupId, req.Keyword);
            var items = await _materialArticleRepository.GetAllAsync(req.GroupId, req.Keyword, req.Page, req.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpGet, Route(RouteId)]
        public async Task<ActionResult<MaterialArticle>> Get([FromQuery]int id)
        {
            return await _materialArticleRepository.GetAsync(id);
        }
    }
}
