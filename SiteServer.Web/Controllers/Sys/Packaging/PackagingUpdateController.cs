using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesUpdateController : ApiController
    {
        [HttpPost, Route(ApiRouteUpdate.Route)]
        public IHttpActionResult Main()
        {
            var request = new Request();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var packageId = request.GetPostString("packageId");
            var version = request.GetPostString("version");

            string errorMessage;
            var idWithVersion = $"{packageId}.{version}";
            var isSsCms = StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSsCms);
            if (!PackageUtils.UpdatePackage(idWithVersion, isSsCms, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            return Ok();
        }
    }
}
