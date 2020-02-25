using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteActionsConnect)]
        public async Task<ActionResult<ConnectResult>> Connect(ConnectRequest request)
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
                return this.Error(errorMessage);
            }

            return result;
        }
    }
}
