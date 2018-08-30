using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesSyncDatabaseController : ApiController
    {
        [HttpPost, Route(ApiRouteSyncDatabase.Route)]
        public IHttpActionResult Main()
        {
            var isCopyFiles = TranslateUtils.ToBool(CacheDbUtils.GetValueAndRemove(PackageUtils.CacheKeySsCmsIsCopyFiles));

            if (!isCopyFiles)
            {
                return Unauthorized();
            }

            SystemManager.SyncDatabase();

            return Ok(new
            {
                SystemManager.Version
            });
        }
    }
}
