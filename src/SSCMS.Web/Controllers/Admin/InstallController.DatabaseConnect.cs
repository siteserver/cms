using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteDatabaseConnect)]
        public async Task<ActionResult<DatabaseConnectResult>> DatabaseConnect([FromBody]DatabaseConnectRequest request)
        {
            if (!await _configRepository.IsNeedInstallAsync()) return Unauthorized();

            var connectionStringWithoutDatabaseName = InstallUtils.GetDatabaseConnectionString(request.DatabaseType, request.DatabaseHost, request.IsDatabaseDefaultPort, TranslateUtils.ToInt(request.DatabasePort), request.DatabaseUserName, request.DatabasePassword, string.Empty);

            var db = new Database(request.DatabaseType, connectionStringWithoutDatabaseName);

            var (isConnectionWorks, message) = await db.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                return this.Error(message);
            }

            var databaseNames = await db.GetDatabaseNamesAsync();

            return new DatabaseConnectResult
            {
                DatabaseNames = databaseNames
            };
        }
    }
}
