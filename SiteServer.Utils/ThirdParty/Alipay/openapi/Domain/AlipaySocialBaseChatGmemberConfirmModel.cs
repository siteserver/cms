using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySocialBaseChatGmemberConfirmModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySocialBaseChatGmemberConfirmModel : AopObject
    {
        /// <summary>
        /// 业务类型，申请接入时和我们申请，用于统计和限流
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 用户所在的群id
        /// </summary>
        [XmlElement("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// 要判断的用户id
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
