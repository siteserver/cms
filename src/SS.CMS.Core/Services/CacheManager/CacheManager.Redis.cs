using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public async Task<(bool IsConnectionWorks, string ErrorMessage)> IsRedisConnectionWorksAsync(string redisConnectionString)
        {
            var retval = false;
            var errorMessage = string.Empty;
            try
            {
                using (var connection = ConnectionMultiplexer.Connect(redisConnectionString))
                {
                    var cache = connection.GetDatabase();

                    string cacheCommand = "PING";
                    var result = await cache.ExecuteAsync(cacheCommand);

                    retval = result != null && result.ToString() == "PONG";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return (retval, errorMessage);
        }
    }
}
