using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public class SettingsManager : ISettingsManager
    {
        private readonly IConfiguration _config;
        public SettingsManager(IConfiguration config, string contentRootPath, string webRootPath)
        {
            _config = config;
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;

            try
            {
                ProductVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

                PluginVersion = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SS.CMS.Abstractions.dll")).ProductVersion;

                if (Assembly.GetEntryAssembly()
                    .GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
                    .SingleOrDefault() is TargetFrameworkAttribute targetFrameworkAttribute)
                {
                    TargetFramework = targetFrameworkAttribute.FrameworkName;
                }
            }
            catch
            {
                // ignored
            }


            AdminUrl = StringUtils.TrimSlash(config.GetValue<string>("AdminUrl"));
            HomeUrl = StringUtils.TrimSlash(config.GetValue<string>("HomeUrl"));
            Language = config.GetValue<string>("Language");
            IsNightlyUpdate = config.GetValue<bool>("IsNightlyUpdate");
            IsProtectData = config.GetValue<bool>("IsProtectData");
            SecretKey = config.GetValue<string>("SecretKey");

            if (string.IsNullOrEmpty(SecretKey))
            {
                SecretKey = StringUtils.GetShortGuid();
            }

            DatabaseType = DatabaseType.Parse(config.GetValue<string>("Database:Type"));
            DatabaseConnectionString = config.GetValue<string>("Database:ConnectionString");
            RedisIsEnabled = config.GetValue<bool>("Redis:IsEnabled");
            RedisConnectionString = config.GetValue<string>("Redis:ConnectionString");

            if (IsProtectData)
            {
                DatabaseConnectionString = Decrypt(DatabaseConnectionString);
                RedisConnectionString = Decrypt(RedisConnectionString);
            }

            var menusPath = PathUtils.GetLangPath(contentRootPath, Language, "menus.yml");
            if (FileUtils.IsFileExists(menusPath))
            {
                Menus = YamlUtils.FileToObject<IList<Menu>>(menusPath);
            }
            var permissionsPath = PathUtils.GetLangPath(contentRootPath, Language, "permissions.yml");
            if (FileUtils.IsFileExists(permissionsPath))
            {
                Permissions = YamlUtils.FileToObject<PermissionsSettings>(permissionsPath);
            }
        }

        public string ContentRootPath { get; }
        public string WebRootPath { get; }
        public string ProductVersion { get; private set; }
        public string PluginVersion { get; private set; }
        public string TargetFramework { get; private set; }
        public bool IsProtectData { get; private set; }
        public string AdminUrl { get; private set; }
        public string HomeUrl { get; private set; }
        public string SecretKey { get; private set; }
        public bool IsNightlyUpdate { get; private set; }
        public string Language { get; private set; }
        public DatabaseType DatabaseType { get; private set; }
        public string DatabaseConnectionString { get; private set; }
        public bool RedisIsEnabled { get; private set; }
        public string RedisConnectionString { get; private set; }

        public IList<Menu> Menus { get; private set; }
        public PermissionsSettings Permissions { get; private set; }

        public string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString, SecretKey);
        }

        public string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString, SecretKey);
        }

        public void SaveSettings(string adminUrl, string homeUrl, string language, bool isProtectData, DatabaseType databaseType, string databaseConnectionString, bool redisIdEnabled, string redisConnectionString)
        {
            AdminUrl = adminUrl;
            HomeUrl = homeUrl;
            Language = language;
            IsProtectData = isProtectData;
            DatabaseType = databaseType;
            DatabaseConnectionString = databaseConnectionString;
            RedisIsEnabled = redisIdEnabled;
            RedisConnectionString = redisConnectionString;

            //             var path = PathUtils.Combine(ContentRootPath, Constants.ConfigFileName);

            //             var databaseConnectionStringValue = databaseConnectionString;
            //             var redisConnectionStringValue = redisConnectionString;
            //             if (isProtectData)
            //             {
            //                 databaseConnectionStringValue = Encrypt(databaseConnectionStringValue);
            //                 redisConnectionStringValue = Encrypt(redisConnectionStringValue);
            //             }

            //             var json = $@"{{
            //   ""AdminUrl"": ""{adminUrl}"",
            //   ""HomeUrl"": ""{homeUrl}"",
            //   ""Language"": ""{language}"",
            //   ""IsNightlyUpdate"": false,
            //   ""IsProtectData"": {isProtectData.ToString().ToLower()},
            //   ""SecretKey"": ""{SecretKey}"",
            //   ""Database"": {{
            //     ""Type"": ""{databaseType.Value}"",
            //     ""ConnectionString"": ""{databaseConnectionStringValue}""
            //   }},
            //   ""Redis"": {{
            //     ""IsEnabled"": {redisIdEnabled.ToString().ToLower()},
            //     ""ConnectionString"": ""{redisConnectionStringValue}""
            //   }}
            // }}
            // ";

            //             await FileUtils.WriteTextAsync(path, json);
        }
    }
}
