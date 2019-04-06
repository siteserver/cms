using System;
using System.IO;
using System.Xml;
using Datory.Utils;

namespace Datory.Tests
{
    public static class EnvUtils
    {
        private const string TestMachine = "DESKTOP-7S7SBTS";

        public static bool IntegrationTestMachine => Environment.MachineName == TestMachine;

        public const string WebConfigFileName = "Web.config";
        /// <summary>
        /// 获取当前正在执行的服务器应用程序的根目录的物理文件系统路径。
        /// </summary>
        public static string PhysicalApplicationPath { get; private set; }

        public static DatabaseType DatabaseType { get; private set; }

        private static string _connectionString;

        public static string ConnectionStringUserId { get; private set; }

        public static string ConnectionString
        {
            get => _connectionString;
            private set
            {
                _connectionString = value;
                ConnectionStringUserId = GetConnectionStringUserId(_connectionString);
            }
        }

        public static void Load(string physicalApplicationPath, string webConfigPath)
        {
            PhysicalApplicationPath = physicalApplicationPath;

            var databaseType = string.Empty;
            var connectionString = string.Empty;
            try
            {
                var doc = new XmlDocument();

                doc.Load(webConfigPath);

                var appSettings = doc.SelectSingleNode("configuration/appSettings");
                if (appSettings != null)
                {
                    foreach (XmlNode setting in appSettings)
                    {
                        if (setting.Name == "add")
                        {
                            var attrKey = setting.Attributes?["key"];
                            if (attrKey != null)
                            {
                                if (ConvertUtils.EqualsIgnoreCase(attrKey.Value, nameof(DatabaseType)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        databaseType = attrValue.Value;
                                    }
                                }
                                else if (ConvertUtils.EqualsIgnoreCase(attrKey.Value, nameof(ConnectionString)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        connectionString = attrValue.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            DatabaseType = DatabaseType.SqlServer;

            if (Equals(DatabaseType.MySql, databaseType))
            {
                DatabaseType = DatabaseType.MySql;
            }
            else if (Equals(DatabaseType.SqlServer, databaseType))
            {
                DatabaseType = DatabaseType.SqlServer;
            }
            else if (Equals(DatabaseType.PostgreSql, databaseType))
            {
                DatabaseType = DatabaseType.PostgreSql;
            }
            else if (Equals(DatabaseType.Oracle, databaseType))
            {
                DatabaseType = DatabaseType.Oracle;
            }

            ConnectionString = GetConnectionString(DatabaseType, connectionString);
        }

        public static bool Equals(DatabaseType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(type.Value.ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static string GetConnectionStringByName(string name)
        {
            var connectionString = string.Empty;
            try
            {
                var doc = new XmlDocument();

                var configFile = Path.Combine(PhysicalApplicationPath, WebConfigFileName);

                doc.Load(configFile);

                var appSettings = doc.SelectSingleNode("configuration/appSettings");
                if (appSettings != null)
                {
                    foreach (XmlNode setting in appSettings)
                    {
                        if (setting.Name != "add") continue;

                        var attrKey = setting.Attributes?["key"];
                        if (attrKey == null) continue;

                        if (!ConvertUtils.EqualsIgnoreCase(attrKey.Value, name)) continue;

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
                if (!ConvertUtils.ContainsIgnoreCase(connectionString, "SslMode="))
                {
                    connectionString += ";SslMode=Preferred;";
                }
                if (!ConvertUtils.ContainsIgnoreCase(connectionString, "CharSet="))
                {
                    connectionString += ";CharSet=utf8;";
                }
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                connectionString = connectionString.TrimEnd(';');
                if (!ConvertUtils.ContainsIgnoreCase(connectionString, "pooling="))
                {
                    connectionString += ";pooling=false;";
                }
            }

            return connectionString;
        }

        public static string GetConnectionStringUserId(string connectionString)
        {
            var userId = string.Empty;

            foreach (var pair in ConvertUtils.StringCollectionToStringList(connectionString, ';'))
            {
                if (!string.IsNullOrEmpty(pair) && pair.IndexOf("=", StringComparison.Ordinal) != -1)
                {
                    var key = pair.Substring(0, pair.IndexOf("=", StringComparison.Ordinal));
                    var value = pair.Substring(pair.IndexOf("=", StringComparison.Ordinal) + 1);
                    if (ConvertUtils.EqualsIgnoreCase(key, "Uid") ||
                        ConvertUtils.EqualsIgnoreCase(key, "Username") ||
                        ConvertUtils.EqualsIgnoreCase(key, "User ID"))
                    {
                        return value;
                    }
                }
            }

            return userId;
        }
    }
}