using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using SS.CMS.Data;
using SS.CMS.Services.ISettingsManager;
using SS.CMS.Settings;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public class SettingsManager : ISettingsManager
    {
        private readonly string CacheKey = StringUtils.GetCacheKey(nameof(SettingsManager));
        private readonly object _lockObject = new object();

        public SettingsManager(IConfiguration config, string contentRootPath, string webRootPath)
        {
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;

            try
            {
                ProductVersion = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.CMS.dll")).ProductVersion;
                PluginVersion = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.Plugin.dll")).ProductVersion;

                if (Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
                    .SingleOrDefault() is TargetFrameworkAttribute targetFrameworkAttribute)
                {
                    TargetFramework = targetFrameworkAttribute.FrameworkName;
                }

                EnvironmentVersion = Environment.Version.ToString();

                //DotNetVersion = FileVersionInfo.GetVersionInfo(typeof(Uri).Assembly.Location).ProductVersion;
            }
            catch
            {
                // ignored
            }

            IsProtectData = config.GetValue<bool>("SS:IsProtectData");
            ApiPrefix = StringUtils.TrimSlash(config.GetValue<string>("SS:ApiPrefix"));
            AdminPrefix = StringUtils.TrimSlash(config.GetValue<string>("SS:AdminPrefix"));
            HomePrefix = StringUtils.TrimSlash(config.GetValue<string>("SS:HomePrefix"));
            SecretKey = config.GetValue<string>("SS:SecretKey");
            IsNightlyUpdate = config.GetValue<bool>("SS:IsNightlyUpdate");
            Language = config.GetValue<string>("SS:Language");

            if (string.IsNullOrEmpty(SecretKey))
            {
                SecretKey = StringUtils.GetShortGuid();
            }

            if (IsProtectData)
            {
                DatabaseType = DatabaseType.GetDatabaseType(TranslateUtils.DecryptStringBySecretKey(config.GetValue<string>("SS:Database:Type"), SecretKey));
                DatabaseConnectionString = TranslateUtils.DecryptStringBySecretKey(config.GetValue<string>("SS:Database:ConnectionString"), SecretKey);
                RedisConnectionString = TranslateUtils.DecryptStringBySecretKey(config.GetValue<string>("SS:Redis:ConnectionString"), SecretKey);
            }
            else
            {
                DatabaseType = DatabaseType.GetDatabaseType(config.GetValue<string>("SS:Database:Type"));
                DatabaseConnectionString = config.GetValue<string>("SS:Database:ConnectionString");
                RedisConnectionString = config.GetValue<string>("SS:Redis:ConnectionString");
            }

            var menusPath = PathUtils.GetLangPath(contentRootPath, Language, "menus.yml");
            var permissionsPath = PathUtils.GetLangPath(contentRootPath, Language, "permissions.yml");

            Menus = YamlUtils.FileToObject<IList<Menu>>(menusPath);
            Permissions = YamlUtils.FileToObject<PermissionsSettings>(permissionsPath);
        }

        public string ContentRootPath { get; }

        public string WebRootPath { get; }

        public string ProductVersion { get; }

        public string PluginVersion { get; }

        public string TargetFramework { get; }

        public string EnvironmentVersion { get; }

        public bool IsProtectData { get; }
        public string ApiPrefix { get; }
        public string AdminPrefix { get; }
        public string HomePrefix { get; }
        public string SecretKey { get; }
        public bool IsNightlyUpdate { get; }
        public string Language { get; }
        public DatabaseType DatabaseType { get; }
        public string DatabaseConnectionString { get; }
        public string RedisConnectionString { get; }

        public IList<Menu> Menus { get; }
        public PermissionsSettings Permissions { get; }

        public string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString, SecretKey);
        }

        public string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString, SecretKey);
        }
    }
}
