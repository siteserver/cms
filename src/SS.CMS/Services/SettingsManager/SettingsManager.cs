using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Datory;
using Microsoft.Extensions.Configuration;
using SS.CMS.Abstractions;

namespace SS.CMS.Services
{
    public class SettingsManager : ISettingsManager
    {
        private readonly IConfiguration _config;

        public SettingsManager(IConfiguration config, string contentRootPath, string webRootPath)
        {
            _config = config;
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                ProductVersion = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;

                var abstractionsPath = PathUtils.Combine(Path.GetDirectoryName(entryAssembly.Location),
                    Constants.AbstractionsAssemblyName);
                if (FileUtils.IsFileExists(abstractionsPath))
                {
                    PluginVersion = FileVersionInfo.GetVersionInfo(abstractionsPath).ProductVersion;
                }

                if (entryAssembly
                    .GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
                    .SingleOrDefault() is TargetFrameworkAttribute targetFrameworkAttribute)
                {
                    TargetFramework = targetFrameworkAttribute.FrameworkName;
                }
            }

            var menusPath = PathUtils.GetLangPath(contentRootPath, "en", "menus.yml");
            if (FileUtils.IsFileExists(menusPath))
            {
                Menus = YamlUtils.FileToObject<IList<Menu>>(menusPath);
            }

            var permissionsPath = PathUtils.GetLangPath(contentRootPath, "en", "permissions.yml");
            if (FileUtils.IsFileExists(permissionsPath))
            {
                Permissions = YamlUtils.FileToObject<PermissionsSettings>(permissionsPath);
            }
        }

        public string ContentRootPath { get; }
        public string WebRootPath { get; }
        public string ProductVersion { get; }
        public string PluginVersion { get; }
        public string TargetFramework { get; }
        public bool IsNightlyUpdate => _config.GetValue<bool>(nameof(IsNightlyUpdate));
        public bool IsProtectData => _config.GetValue<bool>(nameof(IsProtectData));
        public string AdminDirectory => _config.GetValue<string>(nameof(AdminDirectory)) ?? Constants.DefaultAdminDirectory;
        public string HomeDirectory => _config.GetValue<string>(nameof(HomeDirectory)) ?? Constants.DefaultHomeDirectory;
        public string SecurityKey => _config.GetValue<string>(nameof(SecurityKey)) ?? StringUtils.GetShortGuid();
        public DatabaseType DatabaseType => TranslateUtils.ToEnum(IsProtectData ? Decrypt(_config.GetValue<string>("Database:Type")) : _config.GetValue<string>("Database:Type"), DatabaseType.MySql);
        public string DatabaseConnectionString => IsProtectData ? Decrypt(_config.GetValue<string>("Database:ConnectionString")) : _config.GetValue<string>("Database:ConnectionString");
        public IDatabase Database => new Database(DatabaseType, DatabaseConnectionString);
        public string RedisConnectionString => IsProtectData ? Decrypt(_config.GetValue<string>("Redis:ConnectionString")) : _config.GetValue<string>("Redis:ConnectionString");
        public IRedis Redis => new Redis(RedisConnectionString);

        public IList<Menu> Menus { get; }
        public PermissionsSettings Permissions { get; }

        public string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString, SecurityKey);
        }

        public string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString, SecurityKey);
        }

        public async Task SaveSettingsAsync(bool isNightlyUpdate, bool isProtectData, string adminDirectory,
            string homeDirectory, string securityKey, DatabaseType databaseType, string databaseConnectionString,
            string redisConnectionString)
        {
            var path = PathUtils.Combine(ContentRootPath, Constants.ConfigFileName);

            var type = databaseType.GetValue();
            var databaseConnectionStringValue = databaseConnectionString;
            var redisConnectionStringValue = redisConnectionString;
            if (isProtectData)
            {
                type = Encrypt(type);
                databaseConnectionStringValue = Encrypt(databaseConnectionStringValue);
                redisConnectionStringValue = Encrypt(redisConnectionString);
            }

            var json = $@"
{{
  ""IsNightlyUpdate"": {isNightlyUpdate.ToString().ToLower()},
  ""IsProtectData"": {isProtectData.ToString().ToLower()},
  ""AdminDirectory"": ""{adminDirectory}"",
  ""HomeDirectory"": ""{homeDirectory}"",
  ""SecurityKey"": ""{securityKey}"",
  ""Database"": {{
    ""Type"": ""{type}"",
    ""ConnectionString"": ""{databaseConnectionStringValue}""
  }},
  ""Redis"": {{
    ""ConnectionString"": ""{redisConnectionStringValue}""
  }}
}}";

            await FileUtils.WriteTextAsync(path, json.Trim());
        }
    }
}
