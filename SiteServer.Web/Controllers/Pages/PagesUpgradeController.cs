using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/upgrade")]
    public class PagesUpgradeController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin || !rest.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    return Unauthorized();
                }

                if (SystemManager.IsNeedInstall())
                {
                    return BadRequest("系统未安装，向导被禁用");
                }

        //var installedVersion = SystemManager.Version;

        //var AdminUrl = PageUtils.GetAdminUrl(string.Empty);

        //var DownloadApiUrl = ApiRouteDownload.GetUrl(ApiManager.InnerApiUrl);

        //var UpdateApiUrl = ApiRouteUpdate.GetUrl(ApiManager.InnerApiUrl);

        //var UpdateSsCmsApiUrl = ApiRouteUpdateSsCms.GetUrl(ApiManager.InnerApiUrl);

                return Ok(new
                {
                    Value = true,
                    InstalledVersion = SystemManager.Version,
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
        public IHttpActionResult Upgrade()
        {
            var rest = new Rest(Request);

            var isDownload = TranslateUtils.ToBool(CacheDbUtils.GetValueAndRemove(PackageUtils.CacheKeySsCmsIsDownload));

            if (!isDownload)
            {
                return Unauthorized();
            }

            var version = rest.GetPostString("version");

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