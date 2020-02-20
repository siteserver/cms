using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages
{
    
    [RoutePrefix("pages/install")]
    public partial class PagesInstallController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsConnect = "actions/connect";

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;

        public PagesInstallController(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
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

        [HttpPost, Route(RouteActionsConnect)]
        public async Task<ConnectResult> Connect(ConnectRequest request)
        {
            var result = new ConnectResult
            {
                DatabaseNames = new List<string>()
            };

            var errorMessage = string.Empty;

            var databaseType = request.DatabaseType;
            var port = TranslateUtils.ToInt(request.Port);
            if (string.IsNullOrEmpty(request.Server))
            {
                errorMessage = "数据库主机必须填写。";
            }
            else if (!request.IsDefaultPort && port == 0)
            {
                errorMessage = "数据库端口必须填写。";
            }
            else if (string.IsNullOrEmpty(request.UserName))
            {
                errorMessage = "数据库用户必须填写。";
            }
            else if (databaseType == DatabaseType.Oracle && string.IsNullOrEmpty(request.OracleDatabase))
            {
                errorMessage = "数据库名称必须填写。";
            }
            else
            {
                var connectionStringWithoutDatabaseName = GetConnectionString(databaseType == DatabaseType.Oracle, databaseType, request.Server, request.IsDefaultPort, port, request.UserName, request.Password, string.Empty, request.OracleDatabase, request.OracleIsSid, request.OraclePrivilege);

                var db = new Database(databaseType, connectionStringWithoutDatabaseName);

                var (isConnectionWorks, message) = await db.IsConnectionWorksAsync();
                if (isConnectionWorks)
                {
                    result.DatabaseNames = await db.GetTableNamesAsync();
                }
                else
                {
                    errorMessage = message;
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    errorMessage
                ));
            }

            return result;
        }

        private string GetConnectionString(bool isDatabaseName, DatabaseType databaseType, string server, bool isDefaultPort, int port, string userName, string password, string selectedDatabaseName, string oracleDatabase, bool oracleIsSid, OraclePrivilege oraclePrivilege)
        {
            var databaseName = string.Empty;
            if (isDatabaseName)
            {
                databaseName = databaseType == DatabaseType.Oracle ? oracleDatabase : selectedDatabaseName;
            }
            return WebUtils.GetConnectionString(databaseType, server, isDefaultPort, port, userName, password, databaseName, oracleIsSid, oraclePrivilege);
        }
    }
}