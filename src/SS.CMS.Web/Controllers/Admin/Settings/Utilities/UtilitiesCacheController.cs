using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Settings.Utilities
{
    [Route("admin/settings/utilitiesCache")]
    public partial class UtilitiesCacheController : ControllerBase
    {
        private const string Route = "";

        private readonly ICacheManager<object> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public UtilitiesCacheController(ICacheManager<object> cacheManager, IAuthManager authManager, IDbCacheRepository dbCacheRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _dbCacheRepository = dbCacheRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilitiesCache))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                Configuration = _cacheManager.Configuration
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> ClearCache()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilitiesCache))
            {
                return Unauthorized();
            }

            _cacheManager.Clear();
            CacheUtils.ClearAll();
            await _dbCacheRepository.ClearAsync();
            _cacheManager.Clear();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
