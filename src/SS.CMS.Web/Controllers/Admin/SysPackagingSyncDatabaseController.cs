using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Packaging;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("sys/admin/packaging/sync/database")]
    public partial class SysPackagingSyncDatabaseController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;

        public SysPackagingSyncDatabaseController(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
        }

        [HttpPost, Route(Route)]
        public async Task<SubmitResult> Submit()
        {
            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{_settingsManager.ProductVersion}";
            var packagePath = WebUtils.GetPackagesPath(idWithVersion);
            var homeDirectory = _pathManager.GetHomeDirectoryPath(string.Empty);
            if (!DirectoryUtils.IsDirectoryExists(homeDirectory) || !FileUtils.IsFileExists(PathUtils.Combine(homeDirectory, "config.js")))
            {
                DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), homeDirectory, true);
            }

            await _databaseManager.SyncDatabaseAsync();

            return new SubmitResult
            {
                Version = _settingsManager.ProductVersion
            };
        }
    }
}
