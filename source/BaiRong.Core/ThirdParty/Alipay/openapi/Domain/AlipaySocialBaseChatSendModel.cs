using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySocialBaseChatSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySocialBaseChatSendModel : AopObject
    {
        /// <summary>
        /// 消息简短描述，显示在会话列表上，必填
        /// </summary>
        [XmlElement("biz_memo")]
        public string BizMemo { get; set; }

        /// <summary>
        /// 客户端的消息id，需要全局唯一，必填
        /// </summary>
        [XmlElement("client_msg_id")]
        public string ClientMsgId { get; set; }

        /// <summary>
        /// 点击消息card跳转的地址，选填
        /// </summary>
        [XmlElement("link")]
        public string Link { get; set; }

        /// <summary>
        /// 如果是个人消息，是接收消息者的userid，如果是群消息，是群的id，必填
        /// </summary>
        [XmlElement("receiver_id")]
        public string ReceiverId { get; set; }

        /// <summary>
        /// 接受者的用户类型，支付宝1，群组2，讨论组3，必填
        /// </summary>
        [XmlElement("receiver_usertype")]
        public string ReceiverUsertype { get; set; }

        /// <summary>
        /// 消息体的内容，形式为json字符串，必填  分享模板  {   "title":支付宝聊天,   "desc":"支付宝聊天",   "image":"图片地址",   "thumb":"缩略图地址"  }  文本模板  {           "m":"文本消息"  }
        /// </summary>
        [XmlElement("template_data")]
        public string TemplateData { get; set; }

        /// <summary>
        /// 消息模板的类型，分享SHARE，文本TEXT，图片IMAGE，必填
        /// </summary>
        [XmlElement("template_type")]
        public string TemplateType { get; set; }
    }
}
