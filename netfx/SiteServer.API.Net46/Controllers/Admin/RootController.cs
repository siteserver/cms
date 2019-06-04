using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Services;
using SiteServer.CMS.Services.Admin;

namespace SiteServer.API.Controllers.Admin
{
    [RoutePrefix(AdminRoutes.Prefix)]
    public class RootController : ControllerBase
    {
        [HttpGet, Route(RootService.Route)]
        public IHttpActionResult GetConfig()
        {
            return Run(ServiceProvider.RootService.GetConfig);
        }

        [HttpPost, Route(RootService.Route)]
        public async Task<IHttpActionResult> Create()
        {
            return await Run(ServiceProvider.RootService.Create);
        }

        [HttpPost, Route(RootService.RouteActionsDownload)]
        public IHttpActionResult Download()
        {
            return Run(ServiceProvider.RootService.Download);
        }
    }
}