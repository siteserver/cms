using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public class CreateStatusController : ControllerBase
    {
        private const string Route = "cms/create/createStatus";
        private const string RouteActionsCancel = "cms/create/createStatus/actions/cancel";

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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.CreateStatus))
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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.CreateStatus))
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