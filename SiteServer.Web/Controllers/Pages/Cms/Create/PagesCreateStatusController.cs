using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    
    [RoutePrefix("pages/cms/create/createStatus")]
    public class PagesCreateStatusController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsCancel = "actions/cancel";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<CreateTaskSummary>> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateStatus))
            {
                return Request.Unauthorized<ObjectResult<CreateTaskSummary>>();
            }

            var summary = CreateTaskManager.GetTaskSummary(request.SiteId);

            return new ObjectResult<CreateTaskSummary>
            {
                Value = summary
            };
        }

        [HttpPost, Route(RouteActionsCancel)]
        public async Task<BoolResult> Cancel([FromBody] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateStatus))
            {
                return Request.Unauthorized<BoolResult>();
            }

            CreateTaskManager.ClearAllTask(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}