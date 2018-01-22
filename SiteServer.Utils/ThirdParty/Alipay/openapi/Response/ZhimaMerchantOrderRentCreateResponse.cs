using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// ZhimaMerchantOrderRentCreateResponse.
    /// </summary>
    public class ZhimaMerchantOrderRentCreateResponse : AopResponse
    {
        /// <summary>
        /// 是否准入：  Y-准入  N-不准入
        /// </summary>
        [XmlElement("admit_state")]
        public string AdmitState { get; set; }

        /// <summary>
        /// 商户发起借用服务时，需要在借用结束后返回给商户的参数
        /// </summary>
        [XmlElement("invoke_state")]
        public string InvokeState { get; set; }

        /// <summary>
        /// 芝麻信用借还订单号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 外部订单号，需要唯一，由商户传入，芝麻内部会做幂等控制，格式为：yyyyMMddHHmmss+4位随机数
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 借用者的userId
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
