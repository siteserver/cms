using System.Text;
using System.Xml;
using SS.CMS.Data;

namespace SS.CMS.Utils
{
    public static class AppSettings
    {
        public const string AppSettingsFileName = "appSettings.json";

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

        public static bool IsProtectData { get; private set; }
        public static DatabaseType DatabaseType { get; private set; }

        public static string ConnectionString { get; private set; }

        public static string ApiPrefix { get; private set; }
        public static string AdminDirectory { get; private set; }
        public static string HomeDirectory { get; private set; }
        public static string SecretKey { get; private set; }

        public static bool IsNightlyUpdate { get; private set; }

        public static void LoadJson(string contentRootPath, string webRootPath, string appSettingsPath)
        {
            ApplicationPath = "/";
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;

            var isProtectData = false;
            var databaseType = string.Empty;
            var connectionString = string.Empty;
            try
            {
                var json = FileUtils.ReadText(appSettingsPath);

                var dict = TranslateUtils.JsonGetDictionaryIgnorecase(TranslateUtils.ToDictionary(json));

                foreach (var attrKey in dict.Keys)
                {
                    if (string.IsNullOrEmpty(attrKey)) continue;
                    var value = dict[attrKey];

                    if (StringUtils.EqualsIgnoreCase(attrKey, nameof(IsProtectData)))
                    {
                        isProtectData = TranslateUtils.Cast<bool>(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(attrKey, nameof(DatabaseType)))
                    {
                        databaseType = TranslateUtils.Cast<string>(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(attrKey, nameof(ConnectionString)))
                    {
                        connectionString = TranslateUtils.Cast<string>(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(attrKey, nameof(ApiPrefix)))
                    {
                        ApiPrefix = TranslateUtils.Cast<string>(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(attrKey, nameof(AdminDirectory)))
                    {
                        AdminDirectory = TranslateUtils.Cast<string>(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(attrKey, nameof(HomeDirectory)))
                    {
                        HomeDirectory = TranslateUtils.Cast<string>(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(attrKey, nameof(SecretKey)))
                    {
                        SecretKey = TranslateUtils.Cast<string>(value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(attrKey, nameof(IsNightlyUpdate)))
                    {
                        IsNightlyUpdate = TranslateUtils.Cast<bool>(value);
                    }
                }

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
            DatabaseType = DatabaseTypeUtils.GetEnumType(databaseType);
            ConnectionString = GetConnectionString(DatabaseType, connectionString);
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

        public static string GetConnectionStringByName(string name)
        {
            var connectionString = string.Empty;
            try
            {
                var doc = new XmlDocument();

                var configFile = PathUtils.Combine(ContentRootPath, AppSettingsFileName);

                doc.Load(configFile);

                var appSettings = doc.SelectSingleNode("configuration/appSettings");
                if (appSettings != null)
                {
                    foreach (XmlNode setting in appSettings)
                    {
                        if (setting.Name != "add") continue;

                        var attrKey = setting.Attributes?["key"];
                        if (attrKey == null) continue;

                        if (!StringUtils.EqualsIgnoreCase(attrKey.Value, name)) continue;

                        var attrValue = setting.Attributes["value"];
                        if (attrValue != null)
                        {
                            connectionString = attrValue.Value;
                        }
                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return connectionString;
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

        private static string GetConnectionString(DatabaseType databaseType, string connectionString)
        {
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
    }
}