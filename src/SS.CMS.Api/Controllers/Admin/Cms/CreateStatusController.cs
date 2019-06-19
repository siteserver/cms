using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Common.Create;
using SS.CMS.Services.IUserManager;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin.Cms
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route("admin")]
    public class CreateStatusController : ControllerBase
    {
        public const string Route = "cms/createStatus";
        public const string RouteActionsCancel = "cms/createStatus/actions/cancel";

        private readonly IUserManager _userManager;

        public CreateStatusController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet(Route)]
        public ActionResult Get([FromQuery] int siteId)
        {
            if (!_userManager.HasSitePermissions(siteId, AuthTypes.SitePermissions.Create))
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
            if (!_userManager.HasSitePermissions(siteId, AuthTypes.SitePermissions.Create))
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