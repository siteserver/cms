using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common.Create;

namespace SS.CMS.Api.Controllers.Admin.Cms
{
    [Route("admin")]
    public class CreateStatusController : ControllerBase
    {
        public const string Route = "cms/createStatus";
        public const string RouteActionsCancel = "cms/createStatus/actions/cancel";

        private readonly IIdentity _identity;

        public CreateStatusController(IIdentity identity)
        {
            _identity = identity;
        }

        [HttpGet(Route)]
        public ActionResult Get([FromQuery] int siteId)
        {
            if (!_identity.IsAdminLoggin ||
                !_identity.AdminPermissions.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Create))
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
            if (!_identity.IsAdminLoggin ||
                !_identity.AdminPermissions.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Create))
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