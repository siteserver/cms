using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto.Create;
using SSCMS.Abstractions.Dto.Request;
using SSCMS.Abstractions.Dto.Result;

namespace SSCMS.Controllers.Admin.Cms.Create
{
    
    [Route("admin/cms/create/createStatus")]
    public class CreateStatusController : ControllerBase
    {
        private const string Route = "";
        private const string RouteActionsCancel = "actions/cancel";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public CreateStatusController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<CreateTaskSummary>>> Get([FromQuery] SiteRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateStatus))
            {
                return Unauthorized();
            }

            var summary = _createManager.GetTaskSummary(request.SiteId);

            return new ObjectResult<CreateTaskSummary>
            {
                Value = summary
            };
        }

        [HttpPost, Route(RouteActionsCancel)]
        public async Task<ActionResult<BoolResult>> Cancel([FromBody] SiteRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateStatus))
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