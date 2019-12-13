using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto;

namespace SiteServer.API.Controllers.Pages
{
    
    [RoutePrefix("pages/install")]
    public partial class PagesInstallController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsConnect = "actions/connect";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            if (!await SystemManager.IsNeedInstallAsync())
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    "系统已安装成功，向导被禁用"
                ));
            }

            var rootWritable = false;
            try
            {
                var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "version.txt");
                FileUtils.WriteText(filePath, SystemManager.ProductVersion);

                var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, WebConfigUtils.PhysicalApplicationPath);
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
                var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, "index.html");
                FileUtils.WriteText(filePath, Constants.Html5Empty);

                var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName));
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
                ContentRootPath = WebConfigUtils.PhysicalApplicationPath,
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
            foreach (var oraclePrivilege in TranslateUtils.GetEnums<EOraclePrivilege>())
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

        private string GetConnectionString(bool isDatabaseName, DatabaseType databaseType, string server, bool isDefaultPort, int port, string userName, string password, string selectedDatabaseName, string oracleDatabase, bool oracleIsSid, EOraclePrivilege oraclePrivilege)
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