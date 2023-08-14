using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.Common.Material
{
    public partial class LayerFileSelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromQuery] QueryRequest request)
        {
            IEnumerable<MaterialGroup> groups;
            int count;
            IEnumerable<MaterialFile> items;

            var config = await _configRepository.GetAsync();
            if (config.IsMaterialSiteOnly)
            {
                var group = await _materialGroupRepository.GetSiteGroupAsync(MaterialType.File, request.SiteId);
                groups = new List<MaterialGroup>
                {
                    group
                };
                count = await _materialFileRepository.GetCountAsync(group.Id, request.Keyword);
                items = await _materialFileRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage);
            }
            else
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.File);
                count = await _materialFileRepository.GetCountAsync(request.GroupId, request.Keyword);
                items = await _materialFileRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
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
