using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysPackagesUpdateSsCmsController : ApiController
    {
        [HttpPost, Route(ApiRouteUpdateSsCms.Route)]
        public IHttpActionResult Main()
        {
            var request = new RequestImpl();

            var isDownload = TranslateUtils.ToBool(CacheDbUtils.GetValueAndRemove(PackageUtils.CacheKeySsCmsIsDownload));

            if (!isDownload)
            {
                return Unauthorized();
            }

            var version = request.GetPostString("version");

            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{version}";
            var packagePath = PathUtils.GetPackagesPath(idWithVersion);
            var packageWebConfigPath = PathUtils.Combine(packagePath, WebConfigUtils.WebConfigFileName);

            if (!FileUtils.IsFileExists(packageWebConfigPath))
            {
                return BadRequest($"升级包 {WebConfigUtils.WebConfigFileName} 文件不存在");
            }

            WebConfigUtils.UpdateWebConfig(packageWebConfigPath, WebConfigUtils.IsProtectData,
                WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, WebConfigUtils.ApiPrefix, WebConfigUtils.AdminDirectory, WebConfigUtils.HomeDirectory,
                WebConfigUtils.SecretKey, WebConfigUtils.IsNightlyUpdate);

            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFiles.DirectoryName), PathUtils.GetSiteFilesPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteServer.DirectoryName), PathUtils.GetAdminDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), PathUtils.GetHomeDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Bin.DirectoryName), PathUtils.GetBinDirectoryPath(string.Empty), true);
            var isCopyFiles = FileUtils.CopyFile(packageWebConfigPath, PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, WebConfigUtils.WebConfigFileName), true);

            //SystemManager.SyncDatabase();

            return Ok(new
            {
                isCopyFiles
            });
        }
    }
}
