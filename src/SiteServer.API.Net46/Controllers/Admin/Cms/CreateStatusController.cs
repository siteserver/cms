using System;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Admin.Create
{
    [RoutePrefix("admin/cms/createStatus")]
    public class CreateStatusController : ControllerBase
    {
        private const string Route = "";
        private const string RouteActionsCancel = "actions/cancel";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = GetRequest();
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
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsCancel)]
        public IHttpActionResult Cancel()
        {
            try
            {
                var request = GetRequest();
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
                return InternalServerError(ex);
            }
        }
    }
}