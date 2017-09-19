using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySocialBaseChatGmsgSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySocialBaseChatGmsgSendModel : AopObject
    {
        /// <summary>
        /// 消息简短描述，显示在会话列表上，必填
        /// </summary>
        [XmlElement("biz_memo")]
        public string BizMemo { get; set; }

        /// <summary>
        /// 消息业务类型，申请接入时和我们申请，用于统计和限流
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 参数描述必须通俗易懂、无错别字、完整。描述的内容请按此格式填写：参数名+是否唯一(如需)+应用场景+枚举值(如有)+如何获取+特殊说明(如有)。如不符合标准终审会驳回，影响上线客户端的消息id，需要全局唯一，必填时间。
        /// </summary>
        [XmlElement("client_msg_id")]
        public string ClientMsgId { get; set; }

        /// <summary>
        /// 投递方式 1: 群里所有人都收到 2:部分人可见,只发给群里的部分人员看到,rangeUsers是接受者的userId列表 3:部分人不可见,只有部分人无法看到,rangeUsers是不投递的userId列表
        /// </summary>
        [XmlElement("delivery_mode")]
        public string DeliveryMode { get; set; }

        /// <summary>
        /// 消息隐藏方案 默认不隐藏 1:上行隐藏 0:下行隐藏,例如 ：A给B发消息   默认(空): A 看到一条上行消息 B看到一条下行消息(消息文本一样)   上行隐藏(1): A给B 发消息 ，A 看不到消息 B看到消息  下行隐藏(0): A给B发消息，A看到消息 ，B 看不到消息
        /// </summary>
        [XmlElement("hidden_side")]
        public string HiddenSide { get; set; }

        /// <summary>
        /// 点击消息card跳转的地址，选填
        /// </summary>
        [XmlElement("link")]
        public string Link { get; set; }

        /// <summary>
        /// 用于在用户客户端没有前台打开情况下，给用户通知提醒，示例值"发来一个红包"最终显示为"${发送者昵称}发来一个红包"
        /// </summary>
        [XmlElement("push_str")]
        public string PushStr { get; set; }

        /// <summary>
        /// 部分投递的用户uid列表
        /// </summary>
        [XmlArray("range_users")]
        [XmlArrayItem("string")]
        public List<string> RangeUsers { get; set; }

        /// <summary>
        /// 群的id，必填
        /// </summary>
        [XmlElement("receiver_id")]
        public string ReceiverId { get; set; }

        /// <summary>
        /// 接受者的用户类型，群组2，讨论组3，必填
        /// </summary>
        [XmlElement("receiver_usertype")]
        public string ReceiverUsertype { get; set; }

        /// <summary>
        /// 模板code值，根据这个值获取对应的模板填充数据协议
        /// </summary>
        [XmlElement("template_code")]
        public string TemplateCode { get; set; }

        /// <summary>
        /// 消息体的内容，形式为json字符串，必填  分享模板  {   "title":支付宝聊天,   "desc":"支付宝聊天",   "image":"图片地址",   "thumb":"缩略图地址"  }  文本模板  {           "m":"文本消息"  }
        /// </summary>
        [XmlElement("template_data")]
        public string TemplateData { get; set; }
    }
}
