using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteRedisConnect)]
        public async Task<ActionResult<BoolResult>> RedisConnect([FromBody]RedisConnectRequest request)
        {
            if (!await _configRepository.IsNeedInstallAsync()) return Unauthorized();

            var redisConnectionString = string.Empty;
            if (request.IsRedis)
            {
                redisConnectionString = InstallUtils.GetRedisConnectionString(request.RedisHost, request.IsRedisDefaultPort, request.RedisPort, request.IsSsl, request.RedisPassword);
            }

            var db = new Redis(redisConnectionString);

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
