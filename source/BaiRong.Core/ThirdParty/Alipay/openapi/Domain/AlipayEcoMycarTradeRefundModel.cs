using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarTradeRefundModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarTradeRefundModel : AopObject
    {
        /// <summary>
        /// 渠道平台
        /// </summary>
        [XmlElement("isv")]
        public string Isv { get; set; }

        /// <summary>
        /// 退款金额(分)
        /// </summary>
        [XmlElement("refund_fee")]
        public string RefundFee { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [XmlElement("refund_reason")]
        public string RefundReason { get; set; }

        /// <summary>
        /// 退款交易编号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
