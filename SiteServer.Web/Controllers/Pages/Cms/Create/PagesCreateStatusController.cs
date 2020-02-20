using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Create;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    
    [RoutePrefix("pages/cms/create/createStatus")]
    public class PagesCreateStatusController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsCancel = "actions/cancel";

        private readonly ICreateManager _createManager;

        public PagesCreateStatusController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<CreateTaskSummary>> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateStatus))
            {
                return Request.Unauthorized<ObjectResult<CreateTaskSummary>>();
            }

            var summary = _createManager.GetTaskSummary(request.SiteId);

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

            _createManager.ClearAllTask(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}