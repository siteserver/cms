using System.Xml;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace BaiRong.Core
{
    public class WebConfigUtils
    {
        private const string NameIsProtectData = "IsProtectData";
        private const string NameDatabaseType = "DatabaseType";
        private const string NameConnectionString = "ConnectionString";
        private const string NameAdminDirectory = "AdminDirectory";
        private const string NameSecretKey = "SecretKey";

        /// <summary>
        /// 获取当前正在执行的服务器应用程序的根目录的物理文件系统路径。
        /// </summary>
        public static string PhysicalApplicationPath { get; private set; }

        public static bool IsProtectData { get; private set; }
        public static EDatabaseType DatabaseType { get; private set; }
        public static string ConnectionString { get; private set; }

        public static string AdminDirectory { get; private set; }
        public static string SecretKey { get; private set; }

        private static IDbHelper _helper;
        public static IDbHelper Helper
        {
            get
            {
                if (_helper != null) return _helper;

                if (DatabaseType == EDatabaseType.MySql)
                {
                    _helper = new Data.MySql();
                }
                else
                {
                    _helper = new SqlServer();
                }
                return _helper;
            }
        }

        public static void Load(string physicalApplicationPath)
        {
            PhysicalApplicationPath = physicalApplicationPath;

            var isProtectData = false;
            var databaseType = string.Empty;
            var connectionString = string.Empty;
            try
            {
                var doc = new XmlDocument();

                var configFile = PathUtils.Combine(PhysicalApplicationPath, "web.config");

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
                                if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameIsProtectData))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        isProtectData = TranslateUtils.ToBool(attrValue.Value);
                                    }
                                }
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameDatabaseType))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        databaseType = attrValue.Value;
                                    }
                                }
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameConnectionString))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        connectionString = attrValue.Value;
                                    }
                                }
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameAdminDirectory))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        AdminDirectory = attrValue.Value;
                                    }
                                }
                                else if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameSecretKey))
                                {
                                    var attrValue = setting.Attributes["value"];
                                    if (attrValue != null)
                                    {
                                        SecretKey = attrValue.Value;
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
            DatabaseType = EDatabaseTypeUtils.GetEnumType(databaseType);
            ConnectionString = connectionString;
            if (string.IsNullOrEmpty(AdminDirectory))
            {
                AdminDirectory = "siteserver";
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                SecretKey = "vEnfkn16t8aeaZKG3a4Gl9UUlzf4vgqU9xwh8ZV5";
            }
        }

        public static void UpdateWebConfig(bool isProtectData, EDatabaseType databaseType, string connectionString, string adminDirectory, string secretKey)
        {
            var configFilePath = PathUtils.MapPath("~/web.config");

            var doc = new XmlDocument();
            doc.Load(configFilePath);
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
                            if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameIsProtectData))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = isProtectData.ToString();
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameDatabaseType))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = EDatabaseTypeUtils.GetValue(databaseType);
                                    if (isProtectData)
                                    {
                                        attrValue.Value = TranslateUtils.EncryptStringBySecretKey(attrValue.Value);
                                    }
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameConnectionString))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = connectionString;
                                    if (isProtectData)
                                    {
                                        attrValue.Value = TranslateUtils.EncryptStringBySecretKey(attrValue.Value);
                                    }
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameAdminDirectory))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = adminDirectory;
                                    dirty = true;
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(attrKey.Value, NameAdminDirectory))
                            {
                                var attrValue = setting.Attributes["value"];
                                if (attrValue != null)
                                {
                                    attrValue.Value = adminDirectory;
                                    dirty = true;
                                }
                            }
                        }
                    }
                }
            }

            if (dirty)
            {
                var writer = new XmlTextWriter(configFilePath, System.Text.Encoding.UTF8)
                {
                    Formatting = Formatting.Indented
                };
                doc.Save(writer);
                writer.Flush();
                writer.Close();
            }

            IsProtectData = isProtectData;
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            _helper = null;
        }

        public static string GetConnectionStringByName(string name)
        {
            var connectionString = string.Empty;
            try
            {
                var doc = new XmlDocument();

                var configFile = PathUtils.Combine(PhysicalApplicationPath, "web.config");

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