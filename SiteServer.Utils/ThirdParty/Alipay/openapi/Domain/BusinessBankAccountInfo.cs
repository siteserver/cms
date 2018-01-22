using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BusinessBankAccountInfo Data Structure.
    /// </summary>
    [Serializable]
    public class BusinessBankAccountInfo : AopObject
    {
        /// <summary>
        /// 企业对公账户名称
        /// </summary>
        [XmlElement("business_bank_account_name")]
        public string BusinessBankAccountName { get; set; }

        /// <summary>
        /// 企业对公银行账户号
        /// </summary>
        [XmlElement("business_bank_card_no")]
        public string BusinessBankCardNo { get; set; }

        /// <summary>
        /// 企业对公账户开户行名称
        /// </summary>
        [XmlElement("business_bank_name")]
        public string BusinessBankName { get; set; }

        /// <summary>
        /// 企业对公账户开户行支行全称
        /// </summary>
        [XmlElement("business_bank_sub")]
        public string BusinessBankSub { get; set; }
    }
}
