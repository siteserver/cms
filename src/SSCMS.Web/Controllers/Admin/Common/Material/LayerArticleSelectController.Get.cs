using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    public partial class LayerArticleSelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId))
            {
                return Unauthorized();
            }

            var articleIds = ListUtils.GetIntList(request.ArticleIds);

            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Message);
            var count = await _materialArticleRepository.GetCountAsync(request.GroupId, request.Keyword, articleIds);
            var items = await _materialArticleRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage, articleIds);

            return new GetResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }
    }
}
