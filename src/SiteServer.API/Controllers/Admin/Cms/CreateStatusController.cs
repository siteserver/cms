using System;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SiteServer.API.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Admin.Create
{
    [Route("api/admin/cms/createStatus")]
    public class CreateStatusController : ControllerBase
    {
        private readonly Request request;

        public CreateStatusController(Request req)
        {
            request = req;
        }

        private const string Route = "";
        private const string RouteActionsCancel = "actions/cancel";

        [HttpGet(Route)]
        public ActionResult Get()
        {
            try
            {
                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Create))
                {
                    return Unauthorized();
                }

                var summary = CreateTaskManager.GetTaskSummary(siteId);

                return Ok(new
                {
                    Value = summary
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost(RouteActionsCancel)]
        public ActionResult Cancel()
        {
            try
            {
                var siteId = request.GetPostInt("siteId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Create))
                {
                    return Unauthorized();
                }

                CreateTaskManager.ClearAllTask(siteId);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}