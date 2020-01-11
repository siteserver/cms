using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/createStatus")]
    public class PagesCreateStatusController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsCancel = "actions/cancel";

        [HttpGet, Route(Route)]
        public async Task<GenericResult<CreateTaskSummary>> Get()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateStatus))
            {
                return Request.Unauthorized<GenericResult<CreateTaskSummary>>();
            }

            var siteId = request.SiteId;

            var summary = CreateTaskManager.GetTaskSummary(siteId);

            return new GenericResult<CreateTaskSummary>
            {
                Value = summary
            };
        }

        [HttpPost, Route(RouteActionsCancel)]
        public async Task<IHttpActionResult> Cancel()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId, Constants.SitePermissions.CreateStatus))
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