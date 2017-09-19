using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPointOrderAddResponse.
    /// </summary>
    public class AlipayPointOrderAddResponse : AopResponse
    {
        /// <summary>
        /// 支付宝集分宝发放流水号
        /// </summary>
        [XmlElement("alipay_order_no")]
        public string AlipayOrderNo { get; set; }

        /// <summary>
        /// 此参数已废弃。
        /// </summary>
        [XmlElement("result_code")]
        public bool ResultCode { get; set; }
    }
}
