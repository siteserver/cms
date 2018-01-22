using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicMessageLabelSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicMessageLabelSendModel : AopObject
    {
        /// <summary>
        /// 根据标签圈人的过滤器
        /// </summary>
        [XmlElement("filter")]
        public Filter Filter { get; set; }

        /// <summary>
        /// 发送消息内容，支持文本消息和图文消息
        /// </summary>
        [XmlElement("material")]
        public Material Material { get; set; }
    }
}
