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
        public string ProductVersion { get; }
        public string PluginVersion { get; }
        public string TargetFramework { get; }

        public string AdminUrl => StringUtils.TrimSlash(_config.GetValue<string>(nameof(AdminUrl)));
        public string HomeUrl => StringUtils.TrimSlash(_config.GetValue<string>(nameof(HomeUrl)));
        public string Language => _config.GetValue<string>(nameof(Language));
        public bool IsNightlyUpdate => _config.GetValue<bool>(nameof(IsNightlyUpdate));
        public bool IsProtectData => _config.GetValue<bool>(nameof(IsProtectData));
        public string SecurityKey => _config.GetValue<string>(nameof(SecurityKey));
        public DatabaseType DatabaseType => DatabaseType.Parse(_config.GetValue<string>("Database:Type"));
        public string DatabaseConnectionString => IsProtectData ? Decrypt(_config.GetValue<string>("Database:ConnectionString")) : _config.GetValue<string>("Database:ConnectionString");
        public bool RedisIsEnabled => _config.GetValue<bool>("Redis:IsEnabled");
        public string RedisConnectionString => IsProtectData ? Decrypt(_config.GetValue<string>("Redis:ConnectionString")) : _config.GetValue<string>("Redis:ConnectionString");

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

        public void SaveSettings(string adminUrl, string homeUrl, string language, bool isProtectData, DatabaseType databaseType, string databaseConnectionString, bool redisIdEnabled, string redisConnectionString)
        {
            // AdminUrl = adminUrl;
            // HomeUrl = homeUrl;
            // Language = language;
            // IsProtectData = isProtectData;
            // DatabaseType = databaseType;
            // DatabaseConnectionString = databaseConnectionString;
            // RedisIsEnabled = redisIdEnabled;
            // RedisConnectionString = redisConnectionString;

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
