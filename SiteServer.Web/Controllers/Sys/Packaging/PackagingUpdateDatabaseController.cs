using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesUpdateDatabaseController : ApiController
    {
        [HttpGet, Route(ApiRouteUpdateDatabase.Route)]
        public IHttpActionResult Main()
        {
            var context = new RequestContext();

            if (!context.IsAdminLoggin)
            {
                return Unauthorized();
            }

            SystemManager.UpdateDatabase();

            return Ok();
        }
    }
}
