using System.Configuration.Internal;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Configuration;
using SS.CMS.Data;

namespace SS.CMS.Utils
{
    public static class AppSettings
    {
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
        public static DbContext DbContext { get; private set; }

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
                databaseType = Configuration[$"SS:databaseType"];
                connectionString = Configuration[$"SS:connectionString"];
                ApiPrefix = Configuration[$"SS:{nameof(ApiPrefix)}"];
                AdminDirectory = Configuration[$"SS:{nameof(AdminDirectory)}"];
                HomeDirectory = Configuration[$"SS:{nameof(HomeDirectory)}"];
                SecretKey = Configuration[$"SS:{nameof(SecretKey)}"];
                IsNightlyUpdate = TranslateUtils.ToBool(Configuration[$"SS:{nameof(IsNightlyUpdate)}"]);

                if (isProtectData)
                {
                    databaseType = TranslateUtils.DecryptStringBySecretKey(databaseType);
                    connectionString = TranslateUtils.DecryptStringBySecretKey(connectionString);
                }
            }
            catch
            {
                // ignored
            }

            IsProtectData = isProtectData;
            DbContext = new DbContext(DatabaseType.GetDatabaseType(databaseType), connectionString);
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
    }
}