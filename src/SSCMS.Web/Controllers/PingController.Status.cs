using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers
{
    public partial class PingController
    {
        [HttpGet, Route(RouteStatus)]
        public async Task<StatusResult> Status()
        {
            var name = Environment.GetEnvironmentVariable("CUMPUTERNAME") ?? Environment.GetEnvironmentVariable("HOSTNAME") ?? Dns.GetHostName().ToUpper();
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var (isDatabaseWorks, _) = await _settingsManager.Database.IsConnectionWorksAsync();
            var (isRedisWorks, _) = await _settingsManager.Redis.IsConnectionWorksAsync();

            return new StatusResult
            {
                Name = name,
                Env = env,
                Containerized = _settingsManager.Containerized,
                Version = _settingsManager.Version,
                IsDatabaseWorks = isDatabaseWorks,
                IsRedisWorks = isRedisWorks
            };
        }
    }
}