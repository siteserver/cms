using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// RefundDetail Data Structure.
    /// </summary>
    [Serializable]
    public class RefundDetail : AopObject
    {
        /// <summary>
        /// 交易退款金额
        /// </summary>
        [XmlElement("refund_amount")]
        public string RefundAmount { get; set; }

        /// <summary>
        /// 退款备注
        /// </summary>
        [XmlElement("refund_memo")]
        public string RefundMemo { get; set; }

        /// <summary>
        /// 退分润列表
        /// </summary>
        [XmlArray("refund_royaltys")]
        [XmlArrayItem("refund_royalty_info")]
        public List<RefundRoyaltyInfo> RefundRoyaltys { get; set; }

        /// <summary>
        /// 退补差金额
        /// </summary>
        [XmlElement("refund_suppl_amount")]
        public string RefundSupplAmount { get; set; }

        /// <summary>
        /// 退补差备注
        /// </summary>
        [XmlElement("refund_suppl_memo")]
        public string RefundSupplMemo { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
