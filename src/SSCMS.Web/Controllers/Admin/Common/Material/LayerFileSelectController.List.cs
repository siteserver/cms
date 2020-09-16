using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    public partial class LayerFileSelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromQuery] QueryRequest request)
        {
            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.File);
            var count = await _materialFileRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _materialFileRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }
    }
}
