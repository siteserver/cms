using System;
using Datory;
using Microsoft.Extensions.Configuration;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
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
        public const string EnvRedisHost = "REDIS_HOST";
        public const string EnvRedisPort = "REDIS_PORT";
        public const string EnvRedisSsl = "REDIS_SSL";
        public const string EnvRedisPassword = "REDIS_PASSWORD";
        public const string EnvRedisConnectionString = "REDIS_CONNECTION_STRING";

        public void Reload()
        {
            var envSecurityKey = GetEnvironmentVariable(EnvSecurityKey);

            var envDatabaseType = GetEnvironmentVariable(EnvDatabaseType);
            var envDatabaseHost = GetEnvironmentVariable(EnvDatabaseHost);
            var envDatabasePort = GetEnvironmentVariable(EnvDatabasePort);
            var envDatabaseUser = GetEnvironmentVariable(EnvDatabaseUser);
            var envDatabasePassword = GetEnvironmentVariable(EnvDatabasePassword);
            var envDatabaseName = GetEnvironmentVariable(EnvDatabaseName);
            var envDatabaseConnectionString = GetEnvironmentVariable(EnvDatabaseConnectionString);

            var envRedisHost = GetEnvironmentVariable(EnvRedisHost);
            var envRedisPort = GetEnvironmentVariable(EnvRedisPort);
            var envRedisSsl = GetEnvironmentVariable(EnvRedisSsl);
            var envRedisPassword = GetEnvironmentVariable(EnvRedisPassword);
            var envRedisConnectionString = GetEnvironmentVariable(EnvRedisConnectionString);

            var isEnvironment = IsEnvironment(envSecurityKey, envDatabaseType, envDatabaseConnectionString,
                envDatabaseHost, envDatabaseUser, envDatabasePassword, envDatabaseName);

            if (isEnvironment)
            {
                IsProtectData = false;
                SecurityKey = envSecurityKey;
                DatabaseType = TranslateUtils.ToEnum(envDatabaseType, DatabaseType.MySql);
                DatabaseConnectionString = DatabaseType == DatabaseType.SQLite
                    ? $"Data Source={Constants.LocalDbContainerVirtualPath};Version=3;"
                    : envDatabaseConnectionString;
                if (string.IsNullOrEmpty(DatabaseConnectionString))
                {
                    var port = TranslateUtils.ToInt(envDatabasePort);
                    DatabaseConnectionString = InstallUtils.GetDatabaseConnectionString(DatabaseType, envDatabaseHost,
                        port == 0, port,
                        envDatabaseUser, envDatabasePassword, envDatabaseName);
                }
                RedisConnectionString = envRedisConnectionString;
                if (string.IsNullOrEmpty(RedisConnectionString))
                {
                    var port = TranslateUtils.ToInt(envRedisPort);
                    RedisConnectionString = InstallUtils.GetRedisConnectionString(envRedisHost, 
                        port == 0, port,
                        TranslateUtils.ToBool(envRedisSsl), envRedisPassword);
                }
            }
            else
            {
                IsProtectData = _config.GetValue(nameof(IsProtectData), false);
                SecurityKey = _config.GetValue<string>(nameof(SecurityKey));
                DatabaseType = TranslateUtils.ToEnum(
                    IsProtectData
                        ? Decrypt(_config.GetValue<string>("Database:Type"))
                        : _config.GetValue<string>("Database:Type"), DatabaseType.MySql);
                DatabaseConnectionString = DatabaseType == DatabaseType.SQLite
                    ? $"Data Source={Constants.LocalDbHostVirtualPath};Version=3;"
                    : IsProtectData
                        ? Decrypt(_config.GetValue<string>("Database:ConnectionString"))
                        : _config.GetValue<string>("Database:ConnectionString");
                RedisConnectionString = IsProtectData
                    ? Decrypt(_config.GetValue<string>("Redis:ConnectionString"))
                    : _config.GetValue<string>("Redis:ConnectionString");
            }
        }

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
