using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEbppPdeductPayResponse.
    /// </summary>
    public class AlipayEbppPdeductPayResponse : AopResponse
    {
        /// <summary>
        /// 支付宝代扣协议ID
        /// </summary>
        [XmlElement("agreement_id")]
        public string AgreementId { get; set; }

        /// <summary>
        /// 支付宝订单流水号
        /// </summary>
        [XmlElement("bill_no")]
        public string BillNo { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [XmlElement("extend_field")]
        public string ExtendField { get; set; }

        /// <summary>
        /// 商户代扣业务流水
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 订单支付状态。  0：未知  1：成功  2：失败
        /// </summary>
        [XmlElement("result_status")]
        public string ResultStatus { get; set; }
    }
}
