using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesSyncDatabaseController : ApiController
    {
        [HttpPost, Route(ApiRouteSyncDatabase.Route)]
        public IHttpActionResult Main()
        {
            SystemManager.SyncDatabase();

            return Ok();
        }
    }
}
