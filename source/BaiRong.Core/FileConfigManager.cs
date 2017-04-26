using System.Web.Caching;
using System.Xml;

namespace BaiRong.Core
{
    public class FileConfigManager
    {
        public static readonly string CacheKey = "BaiRong.Core.FileConfigManager";

        public string AdminDirectoryName { get; } = "siteserver";

        public bool IsValidateCode { get; } = true;

        public bool IsFindPassword { get; } = true;

        public string SecretKey { get; } = "vEnfkn16t8aeaZKG3a4Gl9UUlzf4vgqU9xwh8ZV5";

        public static FileConfigManager Instance
        {
            get
            {
                var configManager = CacheUtils.Get(CacheKey) as FileConfigManager;
                if (configManager != null) return configManager;

                try
                {
                    var path = PathUtils.MapPath("~/SiteFiles/Configuration/Configuration.config");

                    var doc = new XmlDocument();
                    doc.Load(path);
                    configManager = new FileConfigManager(doc);
                    CacheUtils.Max(CacheKey, configManager, new CacheDependency(path));
                }
                catch
                {
                    var doc = new XmlDocument();
                    configManager = new FileConfigManager(doc);
                }
                return configManager;
            }
        }

        private FileConfigManager(XmlNode doc)
        {
            if (doc == null) return;

            var coreNode = doc.SelectSingleNode("config/core");

            var attributeCollection = coreNode?.Attributes;

            if (attributeCollection == null) return;

            var xmlAttribute = attributeCollection["adminDirectory"];
            if (!string.IsNullOrEmpty(xmlAttribute?.Value))
            {
                AdminDirectoryName = xmlAttribute.Value.ToLower();
            }

            xmlAttribute = attributeCollection["isValidateCode"];
            if (!string.IsNullOrEmpty(xmlAttribute?.Value))
            {
                IsValidateCode = TranslateUtils.ToBool(xmlAttribute.Value);
            }

            xmlAttribute = attributeCollection["isFindPassword"];
            if (!string.IsNullOrEmpty(xmlAttribute?.Value))
            {
                IsFindPassword = TranslateUtils.ToBool(xmlAttribute.Value);
            }

            xmlAttribute = attributeCollection["secretKey"];
            if (!string.IsNullOrEmpty(xmlAttribute?.Value))
            {
                SecretKey = xmlAttribute.Value;
            }
        }
    }
}
