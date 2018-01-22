using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEbppFacepayBillQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEbppFacepayBillQueryModel : AopObject
    {
        /// <summary>
        /// 支付宝交易流水号(和user_id、user_identity_code三者至少传一个)   （缴税业务：out_order_no/user_id/bill_no都可以不传）
        /// </summary>
        [XmlElement("bill_no")]
        public string BillNo { get; set; }

        /// <summary>
        /// ISV流水号，用于控制幂等，须确保全局唯一。  （缴税业务：可采用{征收机关代码}-{外部申报号}的形式）
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 支付宝用户ID(和user_identity_code、bill_no三者至少传一个)   （缴税业务：out_order_no/user_id/bill_no都可以不传）
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// 用户支付宝付款码 (需使用下单时用的码值，10分钟内有效）(和user_id、bill_no三者至少传一个)  （缴税业务：out_order_no/user_id/bill_no都可以不传）
        /// </summary>
        [XmlElement("user_identity_code")]
        public string UserIdentityCode { get; set; }
    }
}
