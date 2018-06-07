using System.Text;
using System.Xml;
using SiteServer.Plugin;

namespace SiteServer.Utils
{
    public static class WebConfigUtils
    {
        public const string WebConfigFileName = "Web.config";
        /// <summary>
        /// 获取当前正在执行的服务器应用程序的根目录的物理文件系统路径。
        /// </summary>
        public static string PhysicalApplicationPath { get; private set; }

        public static bool IsProtectData { get; private set; }
        public static DatabaseType DatabaseType { get; private set; }

        private static string _connectionString;

        public static string ConnectionStringUserId { get; private set; }

        public static string ConnectionString
        {
            get { return _connectionString; }
            private set
            {
                _connectionString = value;
                ConnectionStringUserId = SqlUtils.GetConnectionStringUserId(_connectionString);
            }
        }

        public static string AdminDirectory { get; private set; }
        public static string SecretKey { get; private set; }

        public static bool IsNightlyUpdate { get; private set; }

        public static void Load(string physicalApplicationPath)
        {
            Load(physicalApplicationPath, WebConfigFileName);
        }

        public static void Load(string physicalApplicationPath, string webConfigFileName)
        {
            PhysicalApplicationPath = physicalApplicationPath;

            var isProtectData = false;
            var databaseType = string.Empty;
            var connectionString = string.Empty;
            try
            {
                var doc = new XmlDocument();

                var configFile = PathUtils.Combine(PhysicalApplicationPath, webConfigFileName);

                doc.Load(configFile);

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
                                if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(IsProtectData)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        isProtectData = TranslateUtils.ToBool(attrValue.Value);
                                    }
                                }
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(DatabaseType)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        databaseType = attrValue.Value;
                                    }
                                }
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(ConnectionString)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        connectionString = attrValue.Value;
                                    }
                                }
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(AdminDirectory)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        AdminDirectory = attrValue.Value;
                                    }
                                }
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(SecretKey)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        SecretKey = attrValue.Value;
                                    }
                                }

                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(IsNightlyUpdate)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        IsNightlyUpdate = TranslateUtils.ToBool(attrValue.Value);
                                    }
                                }
                            }
                        }
                    }

                    if (isProtectData)
                    {
                        databaseType = TranslateUtils.DecryptStringBySecretKey(databaseType);
                        connectionString = TranslateUtils.DecryptStringBySecretKey(connectionString);
                    }
                }
            }
            catch
            {
                // ignored
            }

            IsProtectData = isProtectData;
            DatabaseType = DatabaseTypeUtils.GetEnumType(databaseType);
            ConnectionString = SqlUtils.GetConnectionString(DatabaseType, connectionString);
            if (string.IsNullOrEmpty(AdminDirectory))
            {
                AdminDirectory = "siteserver";
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                SecretKey = StringUtils.GetShortGuid();
                //SecretKey = "vEnfkn16t8aeaZKG3a4Gl9UUlzf4vgqU9xwh8ZV5";
            }
        }

        public static void Load(string physicalApplicationPath, string databaseType, string connectionString)
        {
            PhysicalApplicationPath = physicalApplicationPath;
            DatabaseType = DatabaseTypeUtils.GetEnumType(databaseType);
            ConnectionString = SqlUtils.GetConnectionString(DatabaseType, connectionString);
        }

        public static void ResetWebConfig()
        {
            var configPath = PathUtils.Combine(PhysicalApplicationPath, WebConfigFileName);
            ResetWebConfig(configPath);
        }

        public static void ResetWebConfig(string configPath)
        {
            var content = FileUtils.ReadText(configPath, Encoding.UTF8);
            FileUtils.WriteText(configPath, Encoding.UTF8, content);
        }

        public static void UpdateWebConfig(bool isProtectData, DatabaseType databaseType, string connectionString,
            string adminDirectory, string secretKey, bool isNightlyUpdate)
        {
            connectionString = SqlUtils.GetConnectionString(databaseType, connectionString);

            var configPath = PathUtils.Combine(PhysicalApplicationPath, WebConfigFileName);
            UpdateWebConfig(configPath, isProtectData, databaseType, connectionString, adminDirectory, secretKey, isNightlyUpdate);

            IsProtectData = isProtectData;
            DatabaseType = databaseType;
            ConnectionString = connectionString;
        }

        public static void UpdateWebConfig(string configPath, bool isProtectData, DatabaseType databaseType, string connectionString, string adminDirectory, string secretKey, bool isNightlyUpdate)
        {
            connectionString = SqlUtils.GetConnectionString(databaseType, connectionString);

            var doc = new XmlDocument();
            doc.Load(configPath);
            var dirty = false;
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
                            if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(IsProtectData)))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = isProtectData.ToString();
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(DatabaseType)))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = databaseType.Value;
                                    if (isProtectData)
                                    {
                                        attrValue.Value = TranslateUtils.EncryptStringBySecretKey(attrValue.Value, secretKey);
                                    }
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(ConnectionString)))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = connectionString;
                                    if (isProtectData)
                                    {
                                        attrValue.Value = TranslateUtils.EncryptStringBySecretKey(attrValue.Value, secretKey);
                                    }
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(AdminDirectory)))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = adminDirectory;
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(SecretKey)))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = secretKey;
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(IsNightlyUpdate)))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = isNightlyUpdate.ToString();
                                    dirty = true;
                                }
                            }
                        }
                    }
                }
            }

            if (dirty)
            {
                var writer = new XmlTextWriter(configPath, Encoding.UTF8)
                {
                    Formatting = Formatting.Indented
                };
                doc.Save(writer);
                writer.Flush();
                writer.Close();
            }
        }

        public static string GetConnectionStringByName(string name)
        {
            var connectionString = string.Empty;
            try
            {
                var doc = new XmlDocument();

                var configFile = PathUtils.Combine(PhysicalApplicationPath, WebConfigFileName);

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
    }
}