using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// Material Data Structure.
    /// </summary>
    [Serializable]
    public class Material : AopObject
    {
        /// <summary>
        /// 图文消息子消息项集合，单条消息最多6个子项，否则会发送失败
        /// </summary>
        [XmlArray("articles")]
        [XmlArrayItem("article")]
        public List<Article> Articles { get; set; }

        /// <summary>
        /// 消息类型，text：文本类型，image-text：图文类型。当消息类型为text时，text参数必传，当消息类型为image-text时，articles参数必传
        /// </summary>
        [XmlElement("msg_type")]
        public string MsgType { get; set; }

        /// <summary>
        /// 文本消息内容
        /// </summary>
        [XmlElement("text")]
        public Text Text { get; set; }
    }
}
