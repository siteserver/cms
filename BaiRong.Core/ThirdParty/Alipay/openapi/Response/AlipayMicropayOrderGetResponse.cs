using System;
using System.Xml.Serialization;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMicropayOrderGetResponse.
    /// </summary>
    public class AlipayMicropayOrderGetResponse : AopResponse
    {
        /// <summary>
        /// 冻结订单详情
        /// </summary>
        [XmlElement("micro_pay_order_detail")]
        public MicroPayOrderDetail MicroPayOrderDetail { get; set; }
    }
}
