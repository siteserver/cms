using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Packaging;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("sys/admin/packaging/update/sscms")]
    public partial class SysPackagingUpdateSsCmsController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public SysPackagingUpdateSsCmsController(ISettingsManager settingsManager, IPathManager pathManager, IDbCacheRepository dbCacheRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _dbCacheRepository = dbCacheRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody]SubmitRequest request)
        {
            var isDownload = TranslateUtils.ToBool(await _dbCacheRepository.GetValueAndRemoveAsync(PackageUtils.CacheKeySsCmsIsDownload));

            if (!isDownload)
            {
                return Unauthorized();
            }

            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{request.Version}";
            var packagePath = WebUtils.GetPackagesPath(idWithVersion);
            var packageWebConfigPath = PathUtils.Combine(packagePath, Constants.ConfigFileName);

            if (!FileUtils.IsFileExists(packageWebConfigPath))
            {
                return this.Error($"升级包 {Constants.ConfigFileName} 文件不存在");
            }

            //WebConfigUtils.UpdateWebConfig(packageWebConfigPath, WebConfigUtils.IsProtectData,
            //    WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, WebConfigUtils.RedisConnectionString, WebConfigUtils.AdminDirectory, WebConfigUtils.HomeDirectory,
            //    WebConfigUtils.SecretKey, WebConfigUtils.IsNightlyUpdate);

            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFiles.DirectoryName), WebUtils.GetSiteFilesPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteServer.DirectoryName), _pathManager.GetAdminDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), _pathManager.GetHomeDirectoryPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Bin.DirectoryName), _pathManager.GetBinDirectoryPath(string.Empty), true);
            var isCopyFiles = FileUtils.CopyFile(packageWebConfigPath, PathUtils.Combine(_settingsManager.ContentRootPath, Constants.ConfigFileName), true);

            //SystemManager.SyncDatabase();

            return new SubmitResult
            {
                IsCopyFiles = isCopyFiles
            };
        }
    }
}
