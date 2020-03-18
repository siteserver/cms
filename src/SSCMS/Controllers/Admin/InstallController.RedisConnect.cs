using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Abstractions.Dto.Result;
using SSCMS.Core.Extensions;

namespace SSCMS.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteRedisConnect)]
        public async Task<ActionResult<BoolResult>> RedisConnect([FromBody]RedisConnectRequest request)
        {
            if (!await _configRepository.IsNeedInstallAsync()) return Unauthorized();

            var connectionString = GetRedisConnectionString(request);

            var db = new Redis(connectionString);

            var (isConnectionWorks, message) = await db.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                return this.Error(message);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
