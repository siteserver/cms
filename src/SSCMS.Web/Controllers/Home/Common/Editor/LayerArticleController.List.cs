using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Common.Editor
{
    public partial class LayerArticleController
    {
        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<QueryResult>> List([FromBody] QueryRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Message);
            var count = await _materialArticleRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _materialArticleRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }
    }
}
