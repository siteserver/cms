using System;
using System.Xml;
using SS.CMS.Plugin.Data;
using SS.CMS.Plugin.Data.Utils;

namespace SS.CMS.Plugin.Tests
{
    public static class EnvUtils
    {
        private const string TestMachine = "DESKTOP-7S7SBTS";

        public static bool IntegrationTestMachine => Environment.MachineName == TestMachine;

        public static DatabaseType DatabaseType { get; private set; }

        public static string ConnectionString { get; private set; }

        public static void Load(string webConfigPath)
        {
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
                                if (Utilities.EqualsIgnoreCase(attrKey.Value, nameof(DatabaseType)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        databaseType = attrValue.Value;
                                    }
                                }
                                else if (Utilities.EqualsIgnoreCase(attrKey.Value, nameof(ConnectionString)))
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
            else if (Equals(DatabaseType.SQLite, databaseType))
            {
                DatabaseType = DatabaseType.SQLite;
            }

            ConnectionString = GetConnectionString(DatabaseType, connectionString);
        }

        private static bool Equals(DatabaseType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(type.Value.ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        private static string GetConnectionString(DatabaseType databaseType, string connectionString)
        {
            if (databaseType == DatabaseType.MySql)
            {
                connectionString = connectionString.TrimEnd(';');
                if (!Utilities.ContainsIgnoreCase(connectionString, "SslMode="))
                {
                    connectionString += ";SslMode=Preferred;";
                }
                if (!Utilities.ContainsIgnoreCase(connectionString, "CharSet="))
                {
                    connectionString += ";CharSet=utf8;";
                }
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                connectionString = connectionString.TrimEnd(';');
                if (!Utilities.ContainsIgnoreCase(connectionString, "pooling="))
                {
                    connectionString += ";pooling=false;";
                }
            }

            return connectionString;
        }
    }
}