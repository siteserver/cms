using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Packaging;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/updateSystem")]
    public class PagesUpdateSystemController : ApiController
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;

        public PagesUpdateSystemController(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin || !await request.AdminPermissionsImpl.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            if (await _configRepository.IsNeedInstallAsync())
            {
                return BadRequest("系统未安装，向导被禁用");
            }

            return Ok(new
            {
                Value = true,
                PackageId = PackageUtils.PackageIdSsCms,
                InstalledVersion = _settingsManager.ProductVersion,
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.PluginVersion
            });
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> UpdateSsCms()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var version = request.GetPostString("version");

            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{version}";
            var packagePath = WebUtils.GetPackagesPath(idWithVersion);
            var packageWebConfigPath = PathUtils.Combine(packagePath, WebConfigUtils.WebConfigFileName);

            if (!PackageUtils.IsPackageDownload(PackageUtils.PackageIdSsCms, version))
            {
                return BadRequest($"升级包 {idWithVersion} 不存在");
            }

            WebConfigUtils.UpdateWebConfig(packageWebConfigPath, WebConfigUtils.IsProtectData,
                WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, WebConfigUtils.RedisConnectionString, WebConfigUtils.AdminDirectory, WebConfigUtils.HomeDirectory,
                WebConfigUtils.SecretKey, WebConfigUtils.IsNightlyUpdate);

            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFiles.DirectoryName), WebUtils.GetSiteFilesPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteServer.DirectoryName), PathUtility.GetAdminDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), PathUtility.GetHomeDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Bin.DirectoryName), PathUtility.GetBinDirectoryPath(string.Empty), true);
            FileUtils.CopyFile(packageWebConfigPath, PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, WebConfigUtils.WebConfigFileName), true);

            //SystemManager.SyncDatabase();

            return Ok(new
            {
                Value = true
            });
        }
    }
}