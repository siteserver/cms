using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RoutePrepare)]
        public async Task<ActionResult<StringResult>> Prepare([FromBody]PrepareRequest request)
        {
            if (!await _configRepository.IsNeedInstallAsync()) return Unauthorized();

            var (success, errorMessage) =
                await _administratorRepository.InsertValidateAsync(request.UserName, request.AdminPassword, request.Email, request.Mobile);

            if (!success)
            {
                return this.Error(errorMessage);
            }

            if (request.DatabaseType == DatabaseType.SQLite)
            {
                var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.ConfigDirectoryName, Constants.DefaultLocalDbFileName);
                if (!FileUtils.IsFileExists(filePath))
                {
                    await FileUtils.WriteTextAsync(filePath, string.Empty);
                }
            }

            var databaseConnectionString = GetDatabaseConnectionString(true, request.DatabaseType, request.DatabaseHost, request.IsDatabaseDefaultPort, TranslateUtils.ToInt(request.DatabasePort), request.DatabaseUserName, request.DatabasePassword, request.DatabaseName, request.OracleDatabase, request.OracleIsSid, request.OraclePrivilege);
            var redisConnectionString = GetRedisConnectionString(request);

            await _settingsManager.SaveSettingsAsync(false, request.IsProtectData, request.DatabaseType, databaseConnectionString,
                redisConnectionString);

            return new StringResult
            {
                Value = _settingsManager.SecurityKey
            };
        }
    }
}
