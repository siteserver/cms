using System;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/upgrade")]
    public class PagesUpgradeController : ControllerBase
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = GetRequest();
                if (!request.IsAdminLoggin || !request.AdminPermissions.IsSuperAdmin())
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
                    InstalledVersion = SystemManager.ProductVersion,
                    IsNightly = AppSettings.IsNightlyUpdate,
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
            var request = GetRequest();

            var isDownload = TranslateUtils.ToBool(CacheDbUtils.GetValueAndRemove(PackageUtils.CacheKeySsCmsIsDownload));

            if (!isDownload)
            {
                return Unauthorized();
            }

            var version = request.GetPostString("version");

            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{version}";
            var packagePath = PathUtilsEx.GetPackagesPath(idWithVersion);
            var packageWebConfigPath = PathUtils.Combine(packagePath, AppSettings.WebConfigFileName);

            if (!FileUtils.IsFileExists(packageWebConfigPath))
            {
                return BadRequest($"升级包 {AppSettings.WebConfigFileName} 文件不存在");
            }

            AppSettings.UpdateWebConfig(packageWebConfigPath, AppSettings.IsProtectData,
                AppSettings.DatabaseType, AppSettings.ConnectionString, AppSettings.ApiPrefix, AppSettings.AdminDirectory, AppSettings.HomeDirectory,
                AppSettings.SecretKey, AppSettings.IsNightlyUpdate);

            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFiles.DirectoryName), PathUtilsEx.GetSiteFilesPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteServer.DirectoryName), PathUtils.GetAdminDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), PathUtils.GetHomeDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, "Bin"), PathUtils.GetBinDirectoryPath(string.Empty), true);
            var isCopyFiles = FileUtils.CopyFile(packageWebConfigPath, PathUtils.Combine(AppSettings.PhysicalApplicationPath, AppSettings.WebConfigFileName), true);

            //SystemManager.SyncDatabase();

            return Ok(new
            {
                isCopyFiles
            });
        }
    }
}