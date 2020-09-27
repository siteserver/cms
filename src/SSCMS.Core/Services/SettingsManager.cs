using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Datory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class SettingsManager : ISettingsManager
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _config;

        public SettingsManager(IServiceCollection services, IConfiguration config, string contentRootPath, string webRootPath, Assembly entryAssembly)
        {
            _services = services;
            _config = config;
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;

            if (entryAssembly != null)
            {
                Version = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                FrameworkDescription = RuntimeInformation.FrameworkDescription;
                OSDescription = RuntimeInformation.OSDescription;
                string os;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    os = "win";
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    os = "osx";
                }
                else
                {
                    os = "linux";
                }
                var architecture = "x64";
                if (RuntimeInformation.OSArchitecture == Architecture.Arm ||
                    RuntimeInformation.OSArchitecture == Architecture.X86)
                {
                    architecture = "x86";
                }
                OSArchitecture = $"{os}-{architecture}";
                CPUCores = Environment.ProcessorCount;
                Containerized = RunningInContainer;
                Reload();
            }
        }

        public IServiceProvider BuildServiceProvider()
        {
            return _services.BuildServiceProvider();
        }

        public IConfiguration Configuration { get; set; }
        public string ContentRootPath { get; }
        public string WebRootPath { get; }
        public string Version { get; }
        public string FrameworkDescription { get; }
        public string OSArchitecture { get; set; }
        public string OSDescription { get; }
        public bool Containerized { get; }
        public int CPUCores { get; }

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
                RedisConnectionString = envRedisConnectionString;
                if (string.IsNullOrEmpty(DatabaseConnectionString))
                {
                    var port = TranslateUtils.ToInt(envDatabasePort);
                    DatabaseConnectionString = InstallUtils.GetDatabaseConnectionString(DatabaseType, envDatabaseHost,
                        port == 0, port,
                        envDatabaseUser, envDatabasePassword, envDatabaseName);
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

        public bool IsProtectData { get; private set; }

        public string SecurityKey { get; private set; }

        public DatabaseType DatabaseType { get; private set; }

        public string DatabaseConnectionString { get; private set; }

        public string RedisConnectionString { get; private set; }

        public IDatabase Database => new Database(DatabaseType, DatabaseConnectionString);

        public IRedis Redis => new Redis(RedisConnectionString);

        public string ApiHost => _config.GetValue(nameof(ApiHost), "/");

        public bool IsDisablePlugins => _config.GetValue(nameof(IsDisablePlugins), false);

        public string AdminRestrictionHost => _config.GetValue<string>("AdminRestriction:Host");

        public string[] AdminRestrictionAllowList => _config.GetSection("AdminRestriction:AllowList").Get<string[]>();

        public string[] AdminRestrictionBlockList => _config.GetSection("AdminRestriction:BlockList").Get<string[]>();

        public string Encrypt(string inputString, string securityKey = null)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString, !string.IsNullOrEmpty(securityKey) ? securityKey : SecurityKey);
        }

        public string Decrypt(string inputString, string securityKey = null)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString, !string.IsNullOrEmpty(securityKey) ? securityKey : SecurityKey);
        }

        public void SaveSettings(bool isProtectData, bool isDisablePlugins, DatabaseType databaseType, string databaseConnectionString, string redisConnectionString, string adminRestrictionHost, string[] adminRestrictionAllowList, string[] adminRestrictionBlockList)
        {
            var type = databaseType.GetValue();
            var databaseConnectionStringValue = databaseConnectionString;
            var redisConnectionStringValue = redisConnectionString;
            if (isProtectData)
            {
                type = Encrypt(type, SecurityKey);
                databaseConnectionStringValue = Encrypt(databaseConnectionStringValue, SecurityKey);
                redisConnectionStringValue = Encrypt(redisConnectionString, SecurityKey);
            }

            InstallUtils.SaveSettings(ContentRootPath, isProtectData, isDisablePlugins, SecurityKey, type, databaseConnectionStringValue, redisConnectionStringValue, adminRestrictionHost, adminRestrictionAllowList, adminRestrictionBlockList);
            Reload();
        }
    }
}
