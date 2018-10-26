using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys
{
    public class SysPackagesSyncDatabaseController : ApiController
    {
        [HttpPost, Route(ApiRouteSyncDatabase.Route)]
        public IHttpActionResult Main()
        {
            SystemManager.SyncDatabase();

            return Ok(new
            {
                SystemManager.Version
            });
        }
    }
}
