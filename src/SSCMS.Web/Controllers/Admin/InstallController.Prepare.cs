using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
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

            if (_settingsManager.Containerized)
            {
                if (_settingsManager.DatabaseType == DatabaseType.SQLite)
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath,
                        Constants.LocalDbContainerVirtualPath.Substring(1));
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        await FileUtils.WriteTextAsync(filePath, string.Empty);
                    }
                }
            }
            else
            {
                if (request.DatabaseType == DatabaseType.SQLite)
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.LocalDbHostVirtualPath.Substring(1));
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        await FileUtils.WriteTextAsync(filePath, string.Empty);
                    }
                }

                var databaseConnectionString = InstallUtils.GetDatabaseConnectionString(request.DatabaseType, request.DatabaseHost, request.IsDatabaseDefaultPort, TranslateUtils.ToInt(request.DatabasePort), request.DatabaseUserName, request.DatabasePassword, request.DatabaseName);
                var redisConnectionString = string.Empty;
                if (request.IsRedis)
                {
                    redisConnectionString = InstallUtils.GetRedisConnectionString(request.RedisHost, request.IsRedisDefaultPort, request.RedisPort, request.IsSsl, request.RedisPassword);
                }

                _settingsManager.SaveSettings(request.IsProtectData, false, request.DatabaseType, databaseConnectionString,
                    redisConnectionString, string.Empty, null, null);
            }

            return new StringResult
            {
                Value = _settingsManager.SecurityKey
            };
        }
    }
}
