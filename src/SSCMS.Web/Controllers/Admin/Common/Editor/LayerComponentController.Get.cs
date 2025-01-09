using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    public partial class LayerComponentController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            IEnumerable<MaterialGroup> groups;
            int count;
            IEnumerable<MaterialComponent> items;

            var config = await _configRepository.GetAsync();
            if (config.IsMaterialSiteOnly)
            {
                var group = await _materialGroupRepository.GetSiteGroupAsync(MaterialType.Component, request.SiteId);
                groups = new List<MaterialGroup>
                {
                    group
                };
                count = await _materialComponentRepository.GetCountAsync(group.Id, request.Keyword);
                items = await _materialComponentRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage);
            }
            else
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.Component);
                count = await _materialComponentRepository.GetCountAsync(request.GroupId, request.Keyword);
                items = await _materialComponentRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
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
