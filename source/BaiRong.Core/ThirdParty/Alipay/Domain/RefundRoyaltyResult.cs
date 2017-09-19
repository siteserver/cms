using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// RefundRoyaltyResult Data Structure.
    /// </summary>
    [Serializable]
    public class RefundRoyaltyResult : AopObject
    {
        /// <summary>
        /// 退分润金额
        /// </summary>
        [XmlElement("refund_amount")]
        public string RefundAmount { get; set; }

        /// <summary>
        /// 退分润结果码
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 转入人支付宝账号对应用户ID
        /// </summary>
        [XmlElement("trans_in")]
        public string TransIn { get; set; }

        /// <summary>
        /// 转入人支付宝账号
        /// </summary>
        [XmlElement("trans_in_email")]
        public string TransInEmail { get; set; }

        /// <summary>
        /// 转出人支付宝账号对应用户ID
        /// </summary>
        [XmlElement("trans_out")]
        public string TransOut { get; set; }

        /// <summary>
        /// 转出人支付宝账号
        /// </summary>
        [XmlElement("trans_out_email")]
        public string TransOutEmail { get; set; }
    }
}
