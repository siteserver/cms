using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Sys
{
    
    public class SysPackagesDownloadController : ApiController
    {
        [HttpPost, Route(ApiRouteDownload.Route)]
        public async Task<IHttpActionResult> Main()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

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
                await DataProvider.DbCacheRepository.RemoveAndInsertAsync(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
            }

            return Ok();
        }
    }
}
