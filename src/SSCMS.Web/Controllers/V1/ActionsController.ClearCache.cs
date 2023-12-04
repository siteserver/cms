using System.Threading.Tasks;
using Datory.Caching;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ActionsController
    {
        [OpenApiOperation("清除缓存 API", "清除缓存，使用POST发起请求，请求地址为/api/v1/actions/clearCache。")]
        [HttpPost, Route(RouteClearCache)]
        public async Task<ActionResult<BoolResult>> ClearCache()
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeOthers))
            {
                return Unauthorized();
            }
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
