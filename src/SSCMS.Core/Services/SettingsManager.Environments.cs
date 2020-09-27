using System;
using Datory;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class SettingsManager
    {
        public const string EnvSecurityKey = "SECURITY_KEY";
        public const string EnvDatabaseType = "DATABASE_TYPE";
        public const string EnvDatabaseHost = "DATABASE_HOST";
        public const string EnvDatabasePort = "DATABASE_PORT";
        public const string EnvDatabaseUser = "DATABASE_USER";
        public const string EnvDatabasePassword = "DATABASE_PASSWORD";
        public const string EnvDatabaseName = "DATABASE_NAME";
        public const string EnvDatabaseConnectionString = "DATABASE_CONNECTION_STRING";
        public const string EnvRedisConnectionString = "REDIS_CONNECTION_STRING";

        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable($"{Constants.EnvironmentPrefix}{name}");
        }

        public static bool IsEnvironment(string envSecurityKey, string envDatabaseType, string envDatabaseConnectionString, string envDatabaseHost, string envDatabaseUser, string envDatabasePassword, string envDatabaseName)
        {
            if (string.IsNullOrEmpty(envSecurityKey) || string.IsNullOrEmpty(envDatabaseType)) return false;

            bool isEnvironment;

            var databaseType = TranslateUtils.ToEnum(envDatabaseType, DatabaseType.MySql);
            if (databaseType == DatabaseType.SQLite)
            {
                isEnvironment = true;
            }
            else if (!string.IsNullOrEmpty(envDatabaseConnectionString))
            {
                isEnvironment = true;
            }
            else
            {
                isEnvironment = !string.IsNullOrEmpty(envDatabaseHost) && !string.IsNullOrEmpty(envDatabaseUser) &&
                                !string.IsNullOrEmpty(envDatabasePassword) && !string.IsNullOrEmpty(envDatabaseName);
            }

            return isEnvironment;
        }

        public static bool RunningInContainer =>
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != null;
    }
}
