using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertModifyChannelRequest Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertModifyChannelRequest : AopObject
    {
        /// <summary>
        /// 渠道ID（渠道创建接口中，返回的channelID）
        /// </summary>
        [XmlElement("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 渠道说明
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
