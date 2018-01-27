using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.Utils.Packaging;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesDownloadController : ApiController
    {
        [HttpGet, Route(ApiRouteDownload.Route)]
        public IHttpActionResult Main(string packageId, string version)
        {
            var request = new Request();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            string errorMessage;
            var isSuccess = PackageUtils.InstallPackage(packageId, version, true, out errorMessage);

            return Ok(new
            {
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage
            });
        }
    }
}
