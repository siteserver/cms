using System;
using System.Threading.Tasks;
using SiteServer.Utils;
using StackExchange.Redis;

namespace SiteServer.CMS.Core
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

        public (bool IsConnectionWorks, string ErrorMessage) IsConnectionWorks()
        {
            var isSuccess = false;
            var errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                try
                {
                    RedisResult result;
                    using (var connection = ConnectionMultiplexer.Connect(ConnectionString))
                    {
                        var cache = connection.GetDatabase();

                        var cacheCommand = "PING";
                        result = cache.Execute(cacheCommand);
                    }

                    isSuccess = result != null && result.ToString() == "PONG";
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            if (isSuccess)
            {
                (Host, Port, Password, Database, AllowAdmin) = GetRedisConnectionString(ConnectionString);
            }

            return (isSuccess, errorMessage);
        }

        public static (string Host, int Port, string Password, int Database, bool AllowAdmin) GetRedisConnectionString(string connectionString)
        {
            var host = "localhost";
            var post = 6379;
            var password = string.Empty;
            var database = 0;
            var allowAdmin = true;

            foreach (var pair in TranslateUtils.StringCollectionToStringList(connectionString))
            {
                if (string.IsNullOrEmpty(pair)) continue;

                if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                {
                    var key = pair.Substring(0, pair.IndexOf("=", StringComparison.Ordinal));
                    var value = pair.Substring(pair.IndexOf("=", StringComparison.Ordinal) + 1);

                    if (StringUtils.EqualsIgnoreCase(key, "password"))
                    {
                        password = value;
                    }
                    else if (StringUtils.EqualsIgnoreCase(key, "allowAdmin"))
                    {
                        allowAdmin = TranslateUtils.ToBool(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(key, "database"))
                    {
                        database = TranslateUtils.ToInt(value);
                    }
                }
                else if (pair.IndexOf(":", StringComparison.Ordinal) != -1)
                {
                    host = pair.Substring(0, pair.IndexOf(":", StringComparison.Ordinal));
                    post = TranslateUtils.ToInt(pair.Substring(pair.IndexOf(":", StringComparison.Ordinal) + 1));
                }
                else
                {
                    host = pair;
                }
            }

            return (host, post, password, database, allowAdmin);
        }
    }
}