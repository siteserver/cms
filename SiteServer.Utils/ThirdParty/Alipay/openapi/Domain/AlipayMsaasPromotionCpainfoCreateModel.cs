using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMsaasPromotionCpainfoCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMsaasPromotionCpainfoCreateModel : AopObject
    {
        /// <summary>
        /// 唯一应用
        /// </summary>
        [XmlElement("app_id")]
        public string AppId { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        [XmlElement("app_version")]
        public string AppVersion { get; set; }

        /// <summary>
        /// bundle_id
        /// </summary>
        [XmlElement("bundle_id")]
        public string BundleId { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [XmlElement("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 调试数据
        /// </summary>
        [XmlElement("debug")]
        public string Debug { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        [XmlElement("extend")]
        public string Extend { get; set; }

        /// <summary>
        /// IDFA
        /// </summary>
        [XmlElement("idfa")]
        public string Idfa { get; set; }

        /// <summary>
        /// IOS版本
        /// </summary>
        [XmlElement("ios_version")]
        public string IosVersion { get; set; }

        /// <summary>
        /// MAC
        /// </summary>
        [XmlElement("mac")]
        public string Mac { get; set; }
    }
}
