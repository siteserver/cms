using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("sys/admin/packaging/clear/cache")]
    public class SysPackagesClearCacheController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public SysPackagesClearCacheController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Main()
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin)
            {
                return Unauthorized();
            }

            CacheUtils.ClearAll();
            await DataProvider.DbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
