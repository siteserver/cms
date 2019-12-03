using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Abstractions;

namespace SiteServer.API.Controllers.Sys
{
    
    public class SysPackagesUpdateController : ApiController
    {
        [HttpPost, Route(ApiRouteUpdate.Route)]
        public async Task<IHttpActionResult> Main()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var packageId = request.GetPostString("packageId");
            var version = request.GetPostString("version");
            var packageType = request.GetPostString("packageType");
            if (StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSsCms))
            {
                packageType = PackageType.SsCms.Value;
            }

            string errorMessage;
            var idWithVersion = $"{packageId}.{version}";
            if (!PackageUtils.UpdatePackage(idWithVersion, PackageType.Parse(packageType), out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            return Ok();
        }
    }
}
