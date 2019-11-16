using System;
using System.Text;
using System.Xml;
using Datory;

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
            get => _connectionString;
            private set
            {
                _connectionString = value;
                ConnectionStringUserId = GetConnectionStringUserId(_connectionString);
            }
        }

        public static string ApiPrefix { get; private set; }
        public static string AdminDirectory { get; private set; }
        public static string HomeDirectory { get; private set; }
        public static string SecretKey { get; private set; }

        public static bool IsNightlyUpdate { get; private set; }

        public static void Load(string physicalApplicationPath, string webConfigPath)
        {
            PhysicalApplicationPath = physicalApplicationPath;

            var isProtectData = false;
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
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(ApiPrefix)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        ApiPrefix = attrValue.Value;
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
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(HomeDirectory)))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        HomeDirectory = attrValue.Value;
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
                        databaseType = DecryptStringBySecretKey(databaseType);
                        connectionString = DecryptStringBySecretKey(connectionString);
                    }
                }
            }
            catch
            {
                // ignored
            }

            IsProtectData = isProtectData;
            DatabaseType = DatabaseType.Parse(databaseType);
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

        public static void UpdateWebConfig(bool isProtectData, DatabaseType databaseType, string connectionString, string apiPrefix, string adminDirectory, string homeDirectory, string secretKey, bool isNightlyUpdate)
        {
            connectionString = GetConnectionString(databaseType, connectionString);

            var configPath = PathUtils.Combine(PhysicalApplicationPath, WebConfigFileName);
            UpdateWebConfig(configPath, isProtectData, databaseType, connectionString, apiPrefix, adminDirectory, homeDirectory, secretKey, isNightlyUpdate);

            IsProtectData = isProtectData;
            DatabaseType = databaseType;
            ConnectionString = connectionString;
        }

        public static void UpdateWebConfig(string configPath, bool isProtectData, DatabaseType databaseType, string connectionString, string apiPrefix, string adminDirectory, string homeDirectory, string secretKey, bool isNightlyUpdate)
        {
            connectionString = GetConnectionString(databaseType, connectionString);

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
                                        attrValue.Value = EncryptStringBySecretKey(attrValue.Value, secretKey);
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
                                        attrValue.Value = EncryptStringBySecretKey(attrValue.Value, secretKey);
                                    }
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(ApiPrefix)))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = apiPrefix;
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
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, nameof(HomeDirectory)))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = homeDirectory;
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

        public static string GetConnectionStringUserId(string connectionString)
        {
            var userId = string.Empty;

            foreach (var pair in TranslateUtils.StringCollectionToStringList(connectionString, ';'))
            {
                if (!string.IsNullOrEmpty(pair) && pair.IndexOf("=", StringComparison.Ordinal) != -1)
                {
                    var key = pair.Substring(0, pair.IndexOf("=", StringComparison.Ordinal));
                    var value = pair.Substring(pair.IndexOf("=", StringComparison.Ordinal) + 1);
                    if (StringUtils.EqualsIgnoreCase(key, "Uid") ||
                        StringUtils.EqualsIgnoreCase(key, "Username") ||
                        StringUtils.EqualsIgnoreCase(key, "User ID"))
                    {
                        return value;
                    }
                }
            }

            return userId;
        }

        public const string EncryptStingIndicator = "0secret0";

        public static string EncryptStringBySecretKey(string inputString)
        {
            return EncryptStringBySecretKey(inputString, SecretKey);
        }

        public static string EncryptStringBySecretKey(string inputString, string secretKey)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;

            var encryptor = new DesEncryptor
            {
                InputString = inputString,
                EncryptKey = secretKey
            };
            encryptor.DesEncrypt();

            var retVal = encryptor.OutString;
            retVal = retVal.Replace("+", "0add0").Replace("=", "0equals0").Replace("&", "0and0").Replace("?", "0question0").Replace("'", "0quote0").Replace("/", "0slash0");

            return retVal + EncryptStingIndicator;
        }

        public static string DecryptStringBySecretKey(string inputString)
        {
            return DecryptStringBySecretKey(inputString, SecretKey);
        }

        private static string DecryptStringBySecretKey(string inputString, string secretKey)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;

            inputString = inputString.Replace(EncryptStingIndicator, string.Empty).Replace("0add0", "+").Replace("0equals0", "=").Replace("0and0", "&").Replace("0question0", "?").Replace("0quote0", "'").Replace("0slash0", "/");

            var encryptor = new DesEncryptor
            {
                InputString = inputString,
                DecryptKey = secretKey
            };
            encryptor.DesDecrypt();

            return encryptor.OutString;
        }
    }
}