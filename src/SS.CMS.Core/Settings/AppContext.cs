using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Settings
{
    public static class AppContext
    {
        public static IDb Db { get; set; }

        //
        // 摘要:
        //     Gets or sets the absolute path to the directory that contains the application
        //     content files.
        public static string ContentRootPath { get; private set; }

        //
        // 摘要:
        //     Gets or sets the absolute path to the directory that contains the web-servable
        //     application content files.
        public static string WebRootPath { get; private set; }

        public static string ApplicationPath { get; private set; }

        public static IConfiguration Configuration { get; private set; }

        public static bool IsProtectData { get; private set; }
        public static DatabaseType DatabaseType { get; private set; }
        public static string ConnectionString { get; private set; }
        public static string ApiPrefix { get; private set; }
        public static string AdminDirectory { get; private set; }
        public static string HomeDirectory { get; private set; }
        public static string SecretKey { get; private set; }

        public static bool IsNightlyUpdate { get; private set; }

        public static void Load(string contentRootPath, string webRootPath, IConfiguration config)
        {
            ApplicationPath = "/";
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;
            Configuration = config;

            var isProtectData = false;
            var databaseType = string.Empty;
            var connectionString = string.Empty;
            try
            {
                isProtectData = TranslateUtils.ToBool(Configuration[$"SS:{nameof(IsProtectData)}"]);
                ApiPrefix = Configuration[$"SS:{nameof(ApiPrefix)}"];
                AdminDirectory = Configuration[$"SS:{nameof(AdminDirectory)}"];
                HomeDirectory = Configuration[$"SS:{nameof(HomeDirectory)}"];
                SecretKey = Configuration[$"SS:{nameof(SecretKey)}"];
                IsNightlyUpdate = TranslateUtils.ToBool(Configuration[$"SS:{nameof(IsNightlyUpdate)}"]);

                databaseType = Configuration["SS:Database:Type"];
                connectionString = Configuration["SS:Database:ConnectionString"];

                if (isProtectData)
                {
                    databaseType = Decrypt(databaseType);
                    connectionString = Decrypt(connectionString);
                }
            }
            catch
            {
                // ignored
            }

            IsProtectData = isProtectData;
            DatabaseType = DatabaseType.GetDatabaseType(databaseType);
            ConnectionString = connectionString;
            if (ApiPrefix == null)
            {
                ApiPrefix = "api";
            }
            if (AdminDirectory == null)
            {
                AdminDirectory = "SiteServer";
            }
            if (HomeDirectory == null)
            {
                HomeDirectory = "Home";
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                SecretKey = StringUtils.GetShortGuid();
                //SecretKey = "vEnfkn16t8aeaZKG3a4Gl9UUlzf4vgqU9xwh8ZV5";
            }
        }

        public static string GetConnectionString(DatabaseType databaseType, string server, bool isDefaultPort, int port, string userName, string password, string database)
        {
            var connectionString = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                connectionString = $"Server={server};";
                if (!isDefaultPort && port > 0)
                {
                    connectionString += $"Port={port};";
                }
                connectionString += $"Uid={userName};Pwd={password};";
                if (!string.IsNullOrEmpty(database))
                {
                    connectionString += $"Database={database};";
                }
                connectionString += "SslMode=Preferred;CharSet=utf8;";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                connectionString = $"Server={server};";
                if (!isDefaultPort && port > 0)
                {
                    connectionString += $"Port={port};";
                }
                connectionString += $"Uid={userName};Pwd={password};";
                if (!string.IsNullOrEmpty(database))
                {
                    connectionString += $"Database={database};";
                }
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                connectionString = $"Host={server};";
                if (!isDefaultPort && port > 0)
                {
                    connectionString += $"Port={port};";
                }
                connectionString += $"Username={userName};Password={password};";
                if (!string.IsNullOrEmpty(database))
                {
                    connectionString += $"Database={database};";
                }
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                port = !isDefaultPort && port > 0 ? port : 1521;
                database = string.IsNullOrEmpty(database)
                    ? string.Empty
                    : $"(CONNECT_DATA=(SERVICE_NAME={database}))";
                connectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={server})(PORT={port})){database});User ID={userName};Password={password};pooling=false;";
            }

            return connectionString;
        }

        public static string GetRootUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, relatedUrl);
        }

        public static string GetAdminUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, AdminDirectory, relatedUrl);
        }

        public static string GetHomeUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, HomeDirectory, relatedUrl);
        }

        public static string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString, SecretKey);
        }

        public static string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString, SecretKey);
        }

        public static string GetAdminPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(ContentRootPath, AdminDirectory), paths);
        }

        public static string GetHomePath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(ContentRootPath, HomeDirectory), paths);
        }

        public static string GetMenusPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.Menus), paths);
        }

        public static string GetBackupFilesPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.BackupFiles), paths);
        }

        public static string GetTemporaryFilesPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles), paths);
        }

        public static string GetSiteTemplatesPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.SiteTemplates), paths);
        }

        public static string PhysicalSiteServerPath => PathUtils.Combine(ContentRootPath, AdminDirectory);

        public static string PhysicalSiteFilesPath => PathUtils.Combine(ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName);

        public static bool IsSystemDirectory(string directoryName)
        {
            if (StringUtils.EqualsIgnoreCase(directoryName, DirectoryUtils.AspnetClient.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, DirectoryUtils.SiteFiles.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, AdminDirectory))
            {
                return true;
            }
            return false;
        }

        public static bool IsWebSiteDirectory(string directoryName)
        {
            return StringUtils.EqualsIgnoreCase(directoryName, "channels")
                   || StringUtils.EqualsIgnoreCase(directoryName, "contents")
                   || StringUtils.EqualsIgnoreCase(directoryName, "Template")
                   || StringUtils.EqualsIgnoreCase(directoryName, "include")
                   || StringUtils.EqualsIgnoreCase(directoryName, "upload");
        }
    }
}