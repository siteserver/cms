using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesDownloadController : ApiController
    {
        [HttpPost, Route(ApiRouteDownload.Route)]
        public IHttpActionResult Main()
        {
            var request = new AuthRequest();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var packageId = request.GetPostString("packageId");
            var version = request.GetPostString("version");

            try
            {
                PackageUtils.DownloadPackage(packageId, version);
            }
            catch
            {
                PackageUtils.DownloadPackage(packageId, version);
            }

            return Ok();
        }
    }
}
