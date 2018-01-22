using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoEduKtBillingQueryResponse.
    /// </summary>
    public class AlipayEcoEduKtBillingQueryResponse : AopResponse
    {
        /// <summary>
        /// NOT_PAY  待缴费  PAYING  支付中  PAY_SUCCESS 支付成功，处理中  BILLING_SUCCESS 缴费成功  TIMEOUT_CLOSED 逾期关闭账单  ISV_CLOSED 账单关闭
        /// </summary>
        [XmlElement("order_status")]
        public string OrderStatus { get; set; }

        /// <summary>
        /// ISV发送账单时输入ISV端的账单号
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }
    }
}
