using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicMessageGroupSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicMessageGroupSendModel : AopObject
    {
        /// <summary>
        /// 图文消息，当msg_type为image-text，该值必须设置，图文消息中的图片建议尺寸 750 x 350px，小于3M，图片支持jpg、png格式
        /// </summary>
        [XmlArray("articles")]
        [XmlArrayItem("article")]
        public List<Article> Articles { get; set; }

        /// <summary>
        /// 用户分组ID
        /// </summary>
        [XmlElement("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// 纯图片消息，暂时不支持，包含url信息，当msg_type为image时，必须设置该值 ，图片尺寸建议为1080x750px，小于3M，图片支持jpg、png格式
        /// </summary>
        [XmlElement("image")]
        public Image Image { get; set; }

        /// <summary>
        /// 消息类型，text表示文本消息，image-text表示图文消息
        /// </summary>
        [XmlElement("msg_type")]
        public string MsgType { get; set; }

        /// <summary>
        /// 文本消息内容，当msg_type为text，必须设置该值，而且必须同时设置标题和内容字段
        /// </summary>
        [XmlElement("text")]
        public Text Text { get; set; }
    }
}
