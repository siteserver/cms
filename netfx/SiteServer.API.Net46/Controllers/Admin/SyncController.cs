using SiteServer.API.Common;
using System.Web.Http;
using SiteServer.CMS.Services;
using SiteServer.CMS.Services.Admin;

namespace SiteServer.API.Controllers.Admin
{
    [RoutePrefix(AdminRoutes.Prefix)]
    public class SyncController : ControllerBase
    {
        [HttpGet, Route(SyncService.Route)]
        public IHttpActionResult Get()
        {
            return Run(ServiceProvider.SyncService.Get);
        }

        [HttpPost, Route(SyncService.Route)]
        public IHttpActionResult Update()
        {
            return Run(ServiceProvider.SyncService.Update);
        }
    }
}