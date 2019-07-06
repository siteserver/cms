using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SS.CMS.Core.Common
{
    public static class RedisManager
    {
        public static async Task<(bool IsConnectionWorks, string ErrorMessage)> IsConnectionWorksAsync(string redisConnectionString)
        {
            var isSuccess = false;
            var errorMessage = string.Empty;
            try
            {
                using (var connection = ConnectionMultiplexer.Connect(redisConnectionString))
                {
                    var cache = connection.GetDatabase();

                    string cacheCommand = "PING";
                    var result = await cache.ExecuteAsync(cacheCommand);

                    isSuccess = result != null && result.ToString() == "PONG";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return (isSuccess, errorMessage);
        }
    }
}