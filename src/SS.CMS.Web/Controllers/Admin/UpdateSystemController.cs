using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Packaging;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("admin/updateSystem")]
    public partial class UpdateSystemController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;

        public UpdateSystemController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin || !await auth.AdminPermissions.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            if (await _configRepository.IsNeedInstallAsync())
            {
                return this.Error("系统未安装，向导被禁用");
            }

            return new GetResult
            {
                Value = true,
                PackageId = PackageUtils.PackageIdSsCms,
                InstalledVersion = _settingsManager.ProductVersion,
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.PluginVersion
            };
        }

        [HttpPost, Route(Route)]
        public ActionResult<BoolResult> UpdateSsCms([FromBody] UpdateRequest request)
        {
            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{request.Version}";
            var packagePath = _pathManager.GetPackagesPath(idWithVersion);
            var packageWebConfigPath = PathUtils.Combine(packagePath, Constants.ConfigFileName);

            if (!PackageUtils.IsPackageDownload(_pathManager, PackageUtils.PackageIdSsCms, request.Version))
            {
                return this.Error($"升级包 {idWithVersion} 不存在");
            }

            //WebConfigUtils.UpdateWebConfig(packageWebConfigPath, WebConfigUtils.IsProtectData,
            //    WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, WebConfigUtils.RedisConnectionString, WebConfigUtils.AdminDirectory, WebConfigUtils.HomeDirectory,
            //    WebConfigUtils.SecretKey, WebConfigUtils.IsNightlyUpdate);

            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFiles.DirectoryName), _pathManager.GetSiteFilesPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteServer.DirectoryName), _pathManager.GetAdminDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), _pathManager.GetHomeDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Bin.DirectoryName), _pathManager.GetBinDirectoryPath(string.Empty), true);
            FileUtils.CopyFile(packageWebConfigPath, PathUtils.Combine(_settingsManager.ContentRootPath, Constants.ConfigFileName), true);

            //SystemManager.SyncDatabase();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}