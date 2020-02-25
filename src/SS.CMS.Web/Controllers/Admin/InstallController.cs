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
    [Route("admin/install")]
    public partial class InstallController : ControllerBase
    {
        private const string Route = "";
        private const string RouteActionsConnect = "actions/connect";

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;

        public InstallController(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
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
                FileUtils.WriteText(filePath, SystemManager.ProductVersion);

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
                var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, "index.html");
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
                ProductVersion = SystemManager.ProductVersion,
                NetVersion = SystemManager.TargetFramework,
                ContentRootPath = _settingsManager.ContentRootPath,
                RootWritable = rootWritable,
                SiteFilesWritable = siteFilesWritable,
                DatabaseTypes = new List<Select<string>>(),
                AdminUrl = PageUtils.GetLoginUrl(),
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