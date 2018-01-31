using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesSyncDatabaseController : ApiController
    {
        [HttpPost, Route(ApiRouteSyncDatabase.Route)]
        public IHttpActionResult Main()
        {
            var request = new Request();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            SystemManager.SyncDatabase();

            return Ok();
        }
    }
}
