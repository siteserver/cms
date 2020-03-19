using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    [Route("sys/admin/packaging/clear/cache")]
    public class SysPackagesClearCacheController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public SysPackagesClearCacheController(IAuthManager authManager, IDbCacheRepository dbCacheRepository)
        {
            _authManager = authManager;
            _dbCacheRepository = dbCacheRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Main()
        {
            

            if (!await _authManager.IsAdminAuthenticatedAsync())
            {
                return Unauthorized();
            }

            CacheUtils.ClearAll();
            await _dbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
