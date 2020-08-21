using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UtilitiesCacheController : ControllerBase
    {
        private const string Route = "settings/utilitiesCache";

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
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUtilitiesCache))
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
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUtilitiesCache))
            {
                return Unauthorized();
            }

            _cacheManager.Clear();
            await _dbCacheRepository.ClearAsync();
            _cacheManager.Clear();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
