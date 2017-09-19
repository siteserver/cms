using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertAdvChannelResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertAdvChannelResponse : AopObject
    {
        /// <summary>
        /// 广告内容模型
        /// </summary>
        [XmlArray("adv_content_list")]
        [XmlArrayItem("kb_advert_adv_content_response")]
        public List<KbAdvertAdvContentResponse> AdvContentList { get; set; }

        /// <summary>
        /// 广告id
        /// </summary>
        [XmlElement("adv_id")]
        public string AdvId { get; set; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        [XmlElement("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [XmlElement("channel_name")]
        public string ChannelName { get; set; }

        /// <summary>
        /// 渠道类型
        /// </summary>
        [XmlElement("channel_type")]
        public string ChannelType { get; set; }
    }
}
