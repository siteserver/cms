using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    public partial class LayerImageSelectController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromBody] QueryRequest request)
        {
            IEnumerable<MaterialGroup> groups;
            int count;
            IEnumerable<MaterialImage> items;

            var config = await _configRepository.GetAsync();
            if (config.IsMaterialSiteOnly)
            {
                var group = await _materialGroupRepository.GetSiteGroupAsync(MaterialType.Image, request.SiteId);
                groups = new List<MaterialGroup>
                {
                    group
                };
                count = await _materialImageRepository.GetCountAsync(group.Id, request.Keyword);
                items = await _materialImageRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage);
            }
            else
            {
              groups = await _materialGroupRepository.GetAllAsync(MaterialType.Image);
              count = await _materialImageRepository.GetCountAsync(request.GroupId, request.Keyword);
              items = await _materialImageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
            }
            
            // var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Image);
            // var count = await _materialImageRepository.GetCountAsync(request.GroupId, request.Keyword);
            // var items = await _materialImageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

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
