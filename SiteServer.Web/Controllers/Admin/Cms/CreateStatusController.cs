using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Admin.Create
{
    [RoutePrefix("admin/cms/createStatus")]
    public class CreateStatusController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsCancel = "actions/cancel";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest(HttpContext.Current.Request);
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
                var request = new AuthenticatedRequest(HttpContext.Current.Request);
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