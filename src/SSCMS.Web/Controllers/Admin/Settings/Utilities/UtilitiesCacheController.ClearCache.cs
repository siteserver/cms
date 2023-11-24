using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using Datory.Caching;

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

            await _dbCacheRepository.ClearAsync();
            var cacheManager = await CachingUtils.GetCacheManagerAsync(_settingsManager.Redis);
            cacheManager.Clear();
            _cacheManager.Clear();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}