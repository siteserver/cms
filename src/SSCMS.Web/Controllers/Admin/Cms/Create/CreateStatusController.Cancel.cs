using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreateStatusController
    {
        [HttpPost, Route(RouteActionsCancel)]
        public async Task<ActionResult<BoolResult>> Cancel([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.CreateStatus))
            {
                return Unauthorized();
            }

            _createManager.ClearAllTask(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
