using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route(Constants.ApiRoute)]
    public partial class InstallController : ControllerBase
    {
        public const string Route = "install";
        private const string RouteDatabaseConnect = "install/actions/databaseConnect";
        private const string RouteRedisConnect = "install/actions/redisConnect";
        private const string RouteInstall = "install/actions/install";
        private const string RoutePrepare = "install/actions/prepare";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public InstallController(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _configRepository.IsNeedInstallAsync())
            {
                return new GetResult
                {
                    Forbidden = true
                };
            }

            var rootWritable = false;
            try
            {
                var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, "version.txt");
                FileUtils.WriteText(filePath, _settingsManager.ProductVersion);

                var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, _settingsManager.ContentRootPath);
                ioPermission.Demand();

                rootWritable = true;
            }
            catch
            {
                // ignored
            }

            var siteFilesWritable = false;
            try
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName, "index.html");
                FileUtils.WriteText(filePath, Constants.Html5Empty);

                var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, PathUtils.Combine(_settingsManager.ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName));
                ioPermission.Demand();

                siteFilesWritable = true;
            }
            catch
            {
                // ignored
            }

            var result = new GetResult
            {
                ProductVersion = _settingsManager.ProductVersion,
                NetVersion = _settingsManager.TargetFramework,
                ContentRootPath = _settingsManager.ContentRootPath,
                WebRootPath = _settingsManager.WebRootPath,
                RootWritable = rootWritable,
                SiteFilesWritable = siteFilesWritable,
                DatabaseTypes = new List<Select<string>>(),
                AdminUrl = _pathManager.GetAdminUrl(LoginController.Route),
                OraclePrivileges = new List<Select<string>>()
            };

            foreach (var databaseType in TranslateUtils.GetEnums<DatabaseType>())
            {
                result.DatabaseTypes.Add(new Select<string>(databaseType));
            }
            foreach (var oraclePrivilege in TranslateUtils.GetEnums<OraclePrivilege>())
            {
                result.OraclePrivileges.Add(new Select<string>(oraclePrivilege));
            }

            return result;
        }
    }
}