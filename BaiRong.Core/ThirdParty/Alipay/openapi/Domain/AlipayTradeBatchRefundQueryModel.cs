using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayTradeBatchRefundQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayTradeBatchRefundQueryModel : AopObject
    {
        /// <summary>
        /// 商户请求批量退款时传递的批次号。  trade_no和batch_no不能同时为空
        /// </summary>
        [XmlElement("batch_no")]
        public string BatchNo { get; set; }

        /// <summary>
        /// 退款明细的支付宝交易号。  trade_no和batch_no不能同时为空
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
