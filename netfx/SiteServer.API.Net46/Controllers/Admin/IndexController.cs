using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Services;
using SiteServer.CMS.Services.Admin;

namespace SiteServer.API.Controllers.Admin
{
    [RoutePrefix(AdminRoutes.Prefix)]
    public class IndexController : ControllerBase
    {
        [HttpGet, Route(IndexService.Route)]
        public IHttpActionResult Get()
        {
            return Run(ServiceProvider.IndexService.Get);
        }

        [HttpGet, Route(IndexService.RouteUnCheckedList)]
        public IHttpActionResult GetUnCheckedList()
        {
            return Run(ServiceProvider.IndexService.GetUnCheckedList);
        }
    }
}