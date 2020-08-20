using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreateStatusController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<CreateTaskSummary>>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.CreateStatus))
            {
                return Unauthorized();
            }

            var summary = _createManager.GetTaskSummary(request.SiteId);

            return new ObjectResult<CreateTaskSummary>
            {
                Value = summary
            };
        }
    }
}
