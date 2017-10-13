using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// EndowmentOrder Data Structure.
    /// </summary>
    [Serializable]
    public class EndowmentOrder : AopObject
    {
        /// <summary>
        /// apply_amount：申购金额，以分为单位
        /// </summary>
        [XmlElement("apply_amount")]
        public string ApplyAmount { get; set; }

        /// <summary>
        /// 订单id，订单的唯一标识，可以用来做幂等
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// pay_time：支付时间，格式：YYYYMMDDHHMISS
        /// </summary>
        [XmlElement("pay_time")]
        public string PayTime { get; set; }

        /// <summary>
        /// ta_requestId:TA的唯一业务流水号
        /// </summary>
        [XmlElement("ta_request_id")]
        public string TaRequestId { get; set; }
    }
}
