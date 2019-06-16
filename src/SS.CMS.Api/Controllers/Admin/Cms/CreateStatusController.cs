using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Common.Create;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin.Cms
{
    [Route("admin")]
    public class CreateStatusController : ControllerBase
    {
        public const string Route = "cms/createStatus";
        public const string RouteActionsCancel = "cms/createStatus/actions/cancel";

        private readonly IIdentityManager _identityManager;

        public CreateStatusController(IIdentityManager identityManager)
        {
            _identityManager = identityManager;
        }

        [HttpGet(Route)]
        public ActionResult Get([FromQuery] int siteId)
        {
            if (!_identityManager.IsAdminLoggin ||
                !_identityManager.AdminPermissions.HasSitePermissions(siteId, Constants.WebSitePermissions.Create))
            {
                return Unauthorized();
            }

            var summary = CreateTaskManager.GetTaskSummary(siteId);

            return Ok(new
            {
                Value = summary
            });
        }

        [HttpPost(RouteActionsCancel)]
        public ActionResult Cancel([FromBody] int siteId)
        {
            if (!_identityManager.IsAdminLoggin ||
                !_identityManager.AdminPermissions.HasSitePermissions(siteId, Constants.WebSitePermissions.Create))
            {
                return Unauthorized();
            }

            CreateTaskManager.ClearAllTask(siteId);

            return Ok(new
            {
                Value = true
            });
        }
    }
}