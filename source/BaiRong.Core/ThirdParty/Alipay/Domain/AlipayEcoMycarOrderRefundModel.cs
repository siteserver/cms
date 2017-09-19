using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarOrderRefundModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarOrderRefundModel : AopObject
    {
        /// <summary>
        /// 退款交易编号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 退款金额（单位：元）
        /// </summary>
        [XmlElement("refund_fee")]
        public string RefundFee { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [XmlElement("refund_reason")]
        public string RefundReason { get; set; }

        /// <summary>
        /// 退款请求编号，针对一笔退款需保证唯一
        /// </summary>
        [XmlElement("req_no")]
        public string ReqNo { get; set; }
    }
}
