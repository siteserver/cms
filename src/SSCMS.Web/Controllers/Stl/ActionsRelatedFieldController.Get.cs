using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsRelatedFieldController
    {
        [HttpPost, Route(Route)]
        public async Task<string> Submit([FromBody] SubmitRequest request)
        {
            var jsonString = await GetRelatedFieldAsync(request.SiteId, request.RelatedFieldId, request.ParentId);
            var call = request.Callback + "(" + jsonString + ")";

            return call;
        }
    }
}
