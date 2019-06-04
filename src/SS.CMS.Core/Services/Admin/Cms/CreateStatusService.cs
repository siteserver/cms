using SS.CMS.Core.Cache;
using SS.CMS.Core.Common.Create;
using SS.CMS.Plugin;

namespace SS.CMS.Core.Services.Admin.Cms
{
    public class CreateStatusService : ServiceBase
    {
        public const string Route = "cms/createStatus";
        public const string RouteActionsCancel = "cms/createStatus/actions/cancel";

        public ResponseResult<object> Get(IRequest request, IResponse response)
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

        public ResponseResult<object> Cancel(IRequest request, IResponse response)
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
    }
}