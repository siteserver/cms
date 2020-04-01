using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    [OpenApiIgnore]
    public class SysPackagesSyncDatabaseController : ApiController
    {
        [HttpPost, Route(ApiRouteSyncDatabase.Route)]
        public IHttpActionResult Main()
        {
            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{SystemManager.ProductVersion}";
            var packagePath = PathUtils.GetPackagesPath(idWithVersion);
            var homeDirectory = PathUtils.GetHomeDirectoryPath(string.Empty);
            if (!DirectoryUtils.IsDirectoryExists(homeDirectory) || !FileUtils.IsFileExists(PathUtils.Combine(homeDirectory, "config.js")))
            {
                DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), homeDirectory, true);
            }

            SystemManager.SyncDatabase();

            return Ok(new
            {
                Version = SystemManager.ProductVersion
            });
        }
    }
}
