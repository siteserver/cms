using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// McardNotifyMessage Data Structure.
    /// </summary>
    [Serializable]
    public class McardNotifyMessage : AopObject
    {
        /// <summary>
        /// 用户提醒信息，按如下格式拼装，需要ISV提供change_reason。  积分变动模板：{change_reason}，您的积分有变动  余额变动模板：{change_reason}，您的余额有变动  等级变更无需提供原因。
        /// </summary>
        [XmlElement("change_reason")]
        public string ChangeReason { get; set; }

        /// <summary>
        /// JSON格式扩展信息，主要是发送消息中的变量
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 消息类型，每种消息都定义了固定消息模板，  POINT_UPDATE：积分变更消息  BALANCE_UPDATE：余额变更消息  LEVEL_UPDATE：等级变更消息
        /// </summary>
        [XmlElement("message_type")]
        public string MessageType { get; set; }
    }
}
