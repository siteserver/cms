using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertChannelResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertChannelResponse : AopObject
    {
        /// <summary>
        /// 渠道ID
        /// </summary>
        [XmlElement("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 渠道状态  EFFECTIVE：有效  INVALID：无效
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// OFFLINE：线下推广
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
