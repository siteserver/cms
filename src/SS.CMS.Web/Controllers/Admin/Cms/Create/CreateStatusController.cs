using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Create;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Cms.Create
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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateStatus))
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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateStatus))
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