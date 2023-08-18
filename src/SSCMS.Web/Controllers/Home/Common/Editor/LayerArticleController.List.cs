using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;
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

            IEnumerable<MaterialGroup> groups;
            int count;
            IEnumerable<MaterialArticle> items;

            var config = await _configRepository.GetAsync();
            if (config.IsMaterialSiteOnly)
            {
                var group = await _materialGroupRepository.GetSiteGroupAsync(MaterialType.Message, request.SiteId);
                groups = new List<MaterialGroup>
                {
                    group
                };
                count = await _materialArticleRepository.GetCountAsync(group.Id, request.Keyword);
                items = await _materialArticleRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage);
            }
            else
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.Message);
                count = await _materialArticleRepository.GetCountAsync(request.GroupId, request.Keyword);
                items = await _materialArticleRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
            }

            return new QueryResult
            {
                IsSiteOnly = config.IsMaterialSiteOnly,
                Groups = groups,
                Count = count,
                Items = items
            };
        }
    }
}
