using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Datory;
using Microsoft.Extensions.Configuration;

namespace SiteServer.Abstractions.Tests
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

            DatabaseType databaseType;
            string connectionString;
            if (IsProtectData)
            {
                databaseType =
                    TranslateUtils.ToEnum(Decrypt(config.GetValue<string>("Database:Type")), DatabaseType.MySql);
                connectionString = Decrypt(config.GetValue<string>("Database:ConnectionString"));
                Redis = new Redis(Decrypt(config.GetValue<string>("Redis:ConnectionString")));
            }
            else
            {
                databaseType = TranslateUtils.ToEnum(config.GetValue<string>("Database:Type"), DatabaseType.MySql);
                connectionString = config.GetValue<string>("Database:ConnectionString");
                Redis = new Redis(config.GetValue<string>("Redis:ConnectionString"));
            }

            Database = new Database(databaseType, GetConnectionString(databaseType, connectionString));

            if (AdminDirectory == null)
            {
                AdminDirectory = "SiteServer";
            }
            if (HomeDirectory == null)
            {
                HomeDirectory = "Home";
            }
            if (string.IsNullOrEmpty(SecurityKey))
            {
                SecurityKey = StringUtils.GetShortGuid();
                //SecretKey = "vEnfkn16t8aeaZKG3a4Gl9UUlzf4vgqU9xwh8ZV5";
            }
        }

        private static string GetConnectionString(DatabaseType databaseType, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                connectionString = connectionString.TrimEnd(';');
                if (!StringUtils.ContainsIgnoreCase(connectionString, "SslMode="))
                {
                    connectionString += ";SslMode=Preferred;";
                }
                if (!StringUtils.ContainsIgnoreCase(connectionString, "CharSet="))
                {
                    connectionString += ";CharSet=utf8;";
                }
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                connectionString = connectionString.TrimEnd(';');
                if (!StringUtils.ContainsIgnoreCase(connectionString, "pooling="))
                {
                    connectionString += ";pooling=false;";
                }
            }

            return connectionString;
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

        public IDatabase Database { get; }

        public IRedis Redis { get; }

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
