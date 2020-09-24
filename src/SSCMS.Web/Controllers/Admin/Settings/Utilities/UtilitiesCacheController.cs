using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
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

        private readonly Services.ICacheManager<object> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public UtilitiesCacheController(Services.ICacheManager<object> cacheManager, IAuthManager authManager, IDbCacheRepository dbCacheRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _dbCacheRepository = dbCacheRepository;
        }

        public class GetResult
        {
            public IReadOnlyCacheManagerConfiguration Configuration { get; set; }
        }
    }
}
