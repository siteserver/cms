using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using Datory.Caching;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesCacheController
    {
        [HttpPost, Route(RouteClearCache)]
        public async Task<ActionResult<BoolResult>> ClearCache()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUtilitiesCache))
            {
                return Unauthorized();
            }

            var temporaryFilesPath = _pathManager.GetTemporaryFilesPath();
            DirectoryUtils.DeleteDirectoryIfExists(temporaryFilesPath);
            DirectoryUtils.CreateDirectoryIfNotExists(temporaryFilesPath);

            // await _dbCacheRepository.ClearAsync();
            // var cacheManager = await CachingUtils.GetCacheManagerAsync(_settingsManager.Redis);
            // cacheManager.Clear();
            // _cacheManager.Clear();
            await _dbCacheRepository.ClearAllExceptAdminSessionsAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}