using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PaymentAbilityPostbackResponse Data Structure.
    /// </summary>
    [Serializable]
    public class PaymentAbilityPostbackResponse : AopObject
    {
        /// <summary>
        /// 错误的订单信息
        /// </summary>
        [XmlElement("error_order")]
        public string ErrorOrder { get; set; }

        /// <summary>
        /// 成功的订单列表,逗号分隔
        /// </summary>
        [XmlElement("order_ids")]
        public string OrderIds { get; set; }
    }
}
