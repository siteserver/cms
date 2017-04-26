using System.Xml.Serialization;

namespace Top.Api.Security
{
    public class TopSecretGetResponse : TopResponse
    {
        /// <summary>
        /// 秘钥值
        /// </summary>
        [XmlElement("secret")]
        public string Secret { get; set; }

        /// <summary>
        /// 秘钥版本号
        /// </summary>
        [XmlElement("secret_version")]
        public long SecretVersion { get; set; }

        /// <summary>
        /// 下次更新秘钥间隔，单位（秒）
        /// </summary>
        [XmlElement("interval")]
        public long Interval { get; set; }

        /// <summary>
        /// 最长有效期，容灾使用，单位（秒）
        /// </summary>
        [XmlElement("max_interval")]
        public long MaxInterval { get; set; }

        /// <summary>
        /// App配置信息
        /// </summary>
        [XmlElement("app_config")]
        public string AppConfig { get; set; }

    }
}
