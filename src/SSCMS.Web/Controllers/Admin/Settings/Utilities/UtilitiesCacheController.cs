using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
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
        private const string RouteClearCache = "settings/utilitiesCache/actions/clearCache";
        private const string RouteRestart = "settings/utilitiesCache/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ICacheManager _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public UtilitiesCacheController(IHostApplicationLifetime hostApplicationLifetime, ICacheManager cacheManager, IAuthManager authManager, IDbCacheRepository dbCacheRepository)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
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
