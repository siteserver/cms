using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PaymentAccountInfo Data Structure.
    /// </summary>
    [Serializable]
    public class PaymentAccountInfo : AopObject
    {
        /// <summary>
        /// 账号名称
        /// </summary>
        [XmlElement("account_name")]
        public string AccountName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [XmlElement("account_no")]
        public string AccountNo { get; set; }

        /// <summary>
        /// 账号类型：ALIPAY(支付宝)，MYBK(网商银行卡)
        /// </summary>
        [XmlElement("account_type")]
        public string AccountType { get; set; }

        /// <summary>
        /// 金额：单位分
        /// </summary>
        [XmlElement("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// 账单备注
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }
    }
}
