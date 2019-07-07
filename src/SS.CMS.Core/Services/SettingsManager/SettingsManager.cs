using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SS.CMS.Data;
using SS.CMS.Enums;
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
        public string SecurityKey => _config.GetValue<string>(nameof(SecurityKey)) ?? StringUtils.GetShortGuid().ToUpper();
        public DatabaseType DatabaseType => DatabaseType.Parse(_config.GetValue<string>("Database:Type"));
        public string DatabaseConnectionString => IsProtectData ? Decrypt(_config.GetValue<string>("Database:ConnectionString")) : _config.GetValue<string>("Database:ConnectionString");
        public CacheType CacheType => CacheType.Parse(_config.GetValue<string>("Cache:Type"));
        public string CacheConnectionString => IsProtectData ? Decrypt(_config.GetValue<string>("Cache:ConnectionString")) : _config.GetValue<string>("Cache:ConnectionString");

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

        public async Task SaveSettingsAsync(bool isNightlyUpdate, bool isProtectData, string securityKey, DatabaseType databaseType, string databaseConnectionString, CacheType cacheType, string cacheConnectionString)
        {
            var path = PathUtils.Combine(ContentRootPath, Constants.ConfigFileName);

            var databaseConnectionStringValue = databaseConnectionString;
            var cacheConnectionStringValue = cacheConnectionString;
            if (isProtectData)
            {
                databaseConnectionStringValue = Encrypt(databaseConnectionStringValue);
                cacheConnectionStringValue = Encrypt(cacheConnectionString);
            }

            var json = $@"
{{
  ""IsNightlyUpdate"": {isNightlyUpdate.ToString().ToLower()},
  ""IsProtectData"": {isProtectData.ToString().ToLower()},
  ""SecurityKey"": ""{securityKey}"",
  ""Database"": {{
    ""Type"": ""{databaseType.Value}"",
    ""ConnectionString"": ""{databaseConnectionStringValue}""
  }},
  ""Redis"": {{
    ""Type"": ""{cacheType.Value}"",
    ""ConnectionString"": ""{cacheConnectionStringValue}""
  }}
}}";

            await FileUtils.WriteTextAsync(path, json.Trim());
        }
    }
}
