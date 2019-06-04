using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Services;
using SiteServer.CMS.Services.Admin.Cms;

namespace SiteServer.API.Controllers.Admin.Create
{
    [RoutePrefix(AdminRoutes.Prefix)]
    public class CreateStatusController : ControllerBase
    {
        [HttpGet, Route(CreateStatusService.Route)]
        public IHttpActionResult Get()
        {
            return Run(ServiceProvider.CreateStatusService.Get);
        }

        [HttpPost, Route(CreateStatusService.RouteActionsCancel)]
        public IHttpActionResult Cancel()
        {
            return Run(ServiceProvider.CreateStatusService.Cancel);
        }
    }
}