using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesDownloadController : ApiController
    {
        [HttpPost, Route(ApiRouteDownload.Route)]
        public IHttpActionResult Main()
        {
            var request = new Request();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var packageId = request.GetPostString("packageId");
            var version = request.GetPostString("version");

            PackageUtils.DownloadPackage(packageId, version);

            return Ok();
        }
    }
}
