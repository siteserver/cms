using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CodeNOList Data Structure.
    /// </summary>
    [Serializable]
    public class CodeNOList : AopObject
    {
        /// <summary>
        /// 金额
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 对应TP活动码
        /// </summary>
        [XmlElement("code_no")]
        public string CodeNo { get; set; }

        /// <summary>
        /// 如果ticket_type为油券，则该字段1:石化，2:石油
        /// </summary>
        [XmlElement("sub_type")]
        public string SubType { get; set; }

        /// <summary>
        /// 券类型,1:油券
        /// </summary>
        [XmlElement("ticket_type")]
        public string TicketType { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        [XmlElement("valid_date")]
        public string ValidDate { get; set; }
    }
}
