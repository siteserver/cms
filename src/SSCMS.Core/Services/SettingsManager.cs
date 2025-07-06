using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Datory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                var version = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
                if (StringUtils.Contains(version, "+"))
                {
                    version = version.Substring(0, version.IndexOf("+"));
                }
                Version = version;
                FrameworkDescription = RuntimeInformation.FrameworkDescription;
                OSDescription = RuntimeInformation.OSDescription;
                string os;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    os = "win";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
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

        public bool IsProtectData { get; private set; }
        public bool IsSafeMode { get; private set; }

        public string SecurityKey { get; private set; }

        public DatabaseType DatabaseType { get; private set; }

        public string DatabaseConnectionString { get; private set; }

        public string RedisConnectionString { get; private set; }

        public int MaxSites { get; private set; }

        public IDatabase Database => new Database(DatabaseType, DatabaseConnectionString);

        public IRedis Redis => new Redis(RedisConnectionString);

        public bool IsDisablePlugins => _config.GetValue(nameof(IsDisablePlugins), false);

        public string AdminRestrictionHost => _config.GetValue<string>("AdminRestriction:Host");

        public string[] AdminRestrictionAllowList => _config.GetSection("AdminRestriction:AllowList").Get<string[]>();

        public string[] AdminRestrictionBlockList => _config.GetSection("AdminRestriction:BlockList").Get<string[]>();

        public bool CorsIsOrigins => _config.GetValue("Cors:IsOrigins", false);

        public string[] CorsOrigins => _config.GetSection("Cors:Origins").Get<string[]>();

        public string Encrypt(string inputString, string securityKey = null)
        {
            return EncryptUtils.Encrypt(inputString, !string.IsNullOrEmpty(securityKey) ? securityKey : SecurityKey);
        }

        public string Decrypt(string inputString, string securityKey = null)
        {
            return EncryptUtils.Decrypt(inputString, !string.IsNullOrEmpty(securityKey) ? securityKey : SecurityKey);
        }

        public void SaveSettings(bool isProtectData, bool isSafeMode, bool isDisablePlugins, DatabaseType databaseType, string databaseConnectionString, string redisConnectionString, string adminRestrictionHost, string[] adminRestrictionAllowList, string[] adminRestrictionBlockList, bool corsIsOrigins, string[] corsOrigins)
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

            InstallUtils.SaveSettings(ContentRootPath, isProtectData, isSafeMode, isDisablePlugins, SecurityKey, type, databaseConnectionStringValue, redisConnectionStringValue, adminRestrictionHost, adminRestrictionAllowList, adminRestrictionBlockList, corsIsOrigins, corsOrigins);
            Reload();
        }

        public void ChangeDatabase(string configFilePath)
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile(configFilePath);
            var config = configurationBuilder.Build();

            var isProtectData = config.GetValue(nameof(IsProtectData), false);
            var securityKey = config.GetValue<string>(nameof(SecurityKey));
            var databaseType = TranslateUtils.ToEnum(isProtectData
                        ? Decrypt(config.GetValue<string>("Database:Type"), securityKey)
                        : config.GetValue<string>("Database:Type"), DatabaseType.MySql);
            var databaseConnectionString = isProtectData
                        ? Decrypt(config.GetValue<string>("Database:ConnectionString"), securityKey)
                        : config.GetValue<string>("Database:ConnectionString");
            var redisConnectionString = isProtectData
                        ? Decrypt(config.GetValue<string>("Redis:ConnectionString"), securityKey)
                        : config.GetValue<string>("Redis:ConnectionString");

            DatabaseType = databaseType;
            DatabaseConnectionString = databaseConnectionString;
            RedisConnectionString = redisConnectionString;
        }
    }
}
