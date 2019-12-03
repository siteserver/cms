using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    
    [RoutePrefix("pages/cms/createStatus")]
    public class PagesCreateStatusController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsCancel = "actions/cancel";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.WebSitePermissions.Create))
                {
                    return Unauthorized();
                }

                var siteId = request.SiteId;

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
        public async Task<IHttpActionResult> Cancel()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId, Constants.WebSitePermissions.Create))
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