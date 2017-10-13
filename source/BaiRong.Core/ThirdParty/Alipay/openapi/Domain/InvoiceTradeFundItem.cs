using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvoiceTradeFundItem Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceTradeFundItem : AopObject
    {
        /// <summary>
        /// 当前支付工具支付的金额
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 支付宝支付工具描述
        /// </summary>
        [XmlElement("payment_tool_name")]
        public string PaymentToolName { get; set; }

        /// <summary>
        /// 支付宝支付工具类型
        /// </summary>
        [XmlElement("payment_tool_type")]
        public string PaymentToolType { get; set; }
    }
}
