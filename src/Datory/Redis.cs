using System;
using System.Threading.Tasks;
using Datory.Utils;
using StackExchange.Redis;

namespace Datory
{
    public class Redis : IRedis
    {
        public Redis(string connectionString)
        {
            if (connectionString == null) return;

            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public string Host { get; private set; }

        public int Port { get; private set; }
        public string Password { get; private set; }
        public int Database { get; private set; }

        public bool AllowAdmin { get; private set; }

        public async Task<(bool IsConnectionWorks, string ErrorMessage)> IsConnectionWorksAsync()
        {
            var isSuccess = false;
            var errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                try
                {
                    using var connection = ConnectionMultiplexer.Connect(ConnectionString);
                    var cache = connection.GetDatabase();

                    var cacheCommand = "PING";
                    var result = await cache.ExecuteAsync(cacheCommand);

                    isSuccess = result != null && result.ToString() == "PONG";
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            if (isSuccess)
            {
                (Host, Port, Password, Database, AllowAdmin) = Utilities.GetRedisConnectionString(ConnectionString);
            }

            return (isSuccess, errorMessage);
        }
    }
}
