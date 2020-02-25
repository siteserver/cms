using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Stl;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsRelatedFieldController : ControllerBase
    {
        [HttpPost, Route(ApiRouteActionsRelatedField.Route)]
        public async Task<string> Submit([FromBody] SubmitRequest request)
        {
            var jsonString = await GetRelatedFieldAsync(request.SiteId, request.RelatedFieldId, request.ParentId);
            var call = request.Callback + "(" + jsonString + ")";

            return call;
        }
    }
}
