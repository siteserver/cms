using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;
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
                count = await _materialArticleRepository.GetCountAsync(group.Id, request.Keyword, articleIds);
                items = await _materialArticleRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage, articleIds);
            }
            else
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.Message);
                count = await _materialArticleRepository.GetCountAsync(request.GroupId, request.Keyword, articleIds);
                items = await _materialArticleRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage, articleIds);
            }


            return new GetResult
            {
                IsSiteOnly = config.IsMaterialSiteOnly,
                Groups = groups,
                Count = count,
                Items = items
            };
        }
    }
}
