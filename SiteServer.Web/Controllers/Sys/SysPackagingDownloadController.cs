using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysPackagesDownloadController : ApiController
    {
        [HttpPost, Route(ApiRouteDownload.Route)]
        public IHttpActionResult Main()
        {
            var request = new RequestImpl();

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

            if (StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSsCms))
            {
                CacheDbUtils.RemoveAndInsert(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
            }

            return Ok();
        }
    }
}
