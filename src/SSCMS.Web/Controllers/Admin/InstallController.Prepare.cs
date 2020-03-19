using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Core.Extensions;
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
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFilesDirectoryName, Constants.DefaultLocalDbFileName);
                if (!FileUtils.IsFileExists(filePath))
                {
                    await FileUtils.WriteTextAsync(filePath, string.Empty);
                }
            }

            var securityKey = StringUtils.GetShortGuid();
            var databaseConnectionString = GetDatabaseConnectionString(true, request.DatabaseType, request.DatabaseHost, request.IsDatabaseDefaultPort, TranslateUtils.ToInt(request.DatabasePort), request.DatabaseUserName, request.DatabasePassword, request.DatabaseName, request.OracleDatabase, request.OracleIsSid, request.OraclePrivilege);
            var redisConnectionString = GetRedisConnectionString(request);

            await _settingsManager.SaveSettingsAsync(false, request.IsProtectData, Constants.DefaultAdminDirectory,
                Constants.DefaultHomeDirectory, securityKey, request.DatabaseType, databaseConnectionString,
                redisConnectionString);

            return new StringResult
            {
                Value = securityKey
            };
        }
    }
}
