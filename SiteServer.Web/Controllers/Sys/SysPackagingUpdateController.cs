using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysPackagesUpdateController : ApiController
    {
        [HttpPost, Route(ApiRouteUpdate.Route)]
        public IHttpActionResult Main()
        {
            var request = new RequestImpl();

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
