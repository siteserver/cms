using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsRelatedFieldController : ControllerBase
    {
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;

        public ActionsRelatedFieldController(IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            _relatedFieldItemRepository = relatedFieldItemRepository;
        }

        [HttpPost, Route("sys/stl/actions/related_field/{siteId}")]
        public async Task<string> Submit([FromBody] SubmitRequest request)
        {
            var jsonString = await GetRelatedFieldAsync(request.SiteId, request.RelatedFieldId, request.ParentId);
            var call = request.Callback + "(" + jsonString + ")";

            return call;
        }
    }
}
