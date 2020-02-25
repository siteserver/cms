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
        public SettingsManager(IConfiguration config, string contentRootPath, string webRootPath)
        {
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

            IsNightlyUpdate = config.GetValue<bool>(nameof(IsNightlyUpdate));
            IsProtectData = config.GetValue<bool>(nameof(IsProtectData));
            AdminDirectory = config.GetValue<string>(nameof(AdminDirectory)) ?? "admin";
            HomeDirectory = config.GetValue<string>(nameof(HomeDirectory)) ?? "home";
            SecurityKey = config.GetValue<string>(nameof(SecurityKey)) ?? StringUtils.GetShortGuid().ToUpper();
            Database = new Database(TranslateUtils.ToEnum(config.GetValue<string>("Database:Type"), DatabaseType.MySql),
                IsProtectData
                    ? Decrypt(config.GetValue<string>("Database:ConnectionString"))
                    : config.GetValue<string>("Database:ConnectionString"));
            Redis = new Redis(IsProtectData
                ? Decrypt(config.GetValue<string>("Redis:ConnectionString"))
                : config.GetValue<string>("Redis:ConnectionString"));
        }

        public string ContentRootPath { get; }
        public string WebRootPath { get; }
        public string ProductVersion { get; }
        public string PluginVersion { get; }
        public string TargetFramework { get; }
        public bool IsNightlyUpdate { get; }
        public bool IsProtectData { get; }
        public string AdminDirectory { get; }
        public string HomeDirectory { get; }
        public string SecurityKey { get; }

        public IDatabase Database { get; set; }

        public IRedis Redis { get; set; }

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

            var databaseConnectionStringValue = databaseConnectionString;
            var redisConnectionStringValue = redisConnectionString;
            if (isProtectData)
            {
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
    ""Type"": ""{databaseType.GetValue()}"",
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
