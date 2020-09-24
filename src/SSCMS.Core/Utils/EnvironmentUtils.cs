using System;
using SSCMS.Configuration;

namespace SSCMS.Core.Utils
{
    public static class EnvironmentUtils
    {
        public const string SecurityKey = "SECURITY_KEY";
        public const string DatabaseType = "DATABASE_TYPE";
        public const string DatabaseHost = "DATABASE_HOST";
        public const string DatabaseUser = "DATABASE_USER";
        public const string DatabasePassword = "DATABASE_PASSWORD";
        public const string DatabaseName = "DATABASE_NAME";
        public const string DatabaseConnectionString = "DATABASE_CONNECTION_STRING";
        public const string RedisConnectionString = "REDIS_CONNECTION_STRING";

        public static string GetValue(string name)
        {
            return Environment.GetEnvironmentVariable($"{Constants.EnvironmentPrefix}{name}");
        }

        public static bool RunningInContainer =>
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != null;
    }
}
