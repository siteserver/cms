using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    [OpenApiIgnore]
    [RoutePrefix("pages/updateSystem")]
    public class PagesUpdateSystemController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin || !request.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    return Unauthorized();
                }

                if (SystemManager.IsNeedInstall())
                {
                    return BadRequest("系统未安装，向导被禁用");
                }

                return Ok(new
                {
                    Value = true,
                    PackageId = PackageUtils.PackageIdSsCms,
                    InstalledVersion = SystemManager.ProductVersion,
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    Version = SystemManager.PluginVersion
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult UpdateSsCms()
        {
            var request = new AuthenticatedRequest();

            var version = request.GetPostString("version");

            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{version}";
            var packagePath = PathUtils.GetPackagesPath(idWithVersion);
            var packageWebConfigPath = PathUtils.Combine(packagePath, WebConfigUtils.WebConfigFileName);

            if (!PackageUtils.IsPackageDownload(PackageUtils.PackageIdSsCms, version))
            {
                return BadRequest($"升级包 {idWithVersion} 不存在");
            }

            WebConfigUtils.UpdateWebConfig(packageWebConfigPath, WebConfigUtils.IsProtectData,
                WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, WebConfigUtils.ApiPrefix, WebConfigUtils.AdminDirectory, WebConfigUtils.HomeDirectory,
                WebConfigUtils.SecretKey, WebConfigUtils.IsNightlyUpdate);

            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFiles.DirectoryName), PathUtils.GetSiteFilesPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteServer.DirectoryName), PathUtils.GetAdminDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), PathUtils.GetHomeDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Bin.DirectoryName), PathUtils.GetBinDirectoryPath(string.Empty), true);
            FileUtils.CopyFile(packageWebConfigPath, PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, WebConfigUtils.WebConfigFileName), true);

            //SystemManager.SyncDatabase();

            return Ok(new
            {
                Value = true
            });
        }
    }
}