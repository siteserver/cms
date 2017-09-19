using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AccountRecord Data Structure.
    /// </summary>
    [Serializable]
    public class AccountRecord : AopObject
    {
        /// <summary>
        /// 支付宝订单号
        /// </summary>
        [XmlElement("alipay_order_no")]
        public string AlipayOrderNo { get; set; }

        /// <summary>
        /// 当时账户的余额
        /// </summary>
        [XmlElement("balance")]
        public string Balance { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [XmlElement("business_type")]
        public string BusinessType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [XmlElement("create_time")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 收入金额
        /// </summary>
        [XmlElement("in_amount")]
        public string InAmount { get; set; }

        /// <summary>
        /// 账务备注说明
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [XmlElement("merchant_order_no")]
        public string MerchantOrderNo { get; set; }

        /// <summary>
        /// 对方支付宝账户ID
        /// </summary>
        [XmlElement("opt_user_id")]
        public string OptUserId { get; set; }

        /// <summary>
        /// 支出金额
        /// </summary>
        [XmlElement("out_amount")]
        public string OutAmount { get; set; }

        /// <summary>
        /// 本方支付宝账户ID
        /// </summary>
        [XmlElement("self_user_id")]
        public string SelfUserId { get; set; }

        /// <summary>
        /// 账务类型
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
