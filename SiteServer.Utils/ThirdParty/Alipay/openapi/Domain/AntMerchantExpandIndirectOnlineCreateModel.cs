using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntMerchantExpandIndirectOnlineCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntMerchantExpandIndirectOnlineCreateModel : AopObject
    {
        /// <summary>
        /// 商户地址信息
        /// </summary>
        [XmlArray("address_info")]
        [XmlArrayItem("address_info")]
        public List<AddressInfo> AddressInfo { get; set; }

        /// <summary>
        /// 商户简称
        /// </summary>
        [XmlElement("alias_name")]
        public string AliasName { get; set; }

        /// <summary>
        /// 签约受理机构pid
        /// </summary>
        [XmlElement("bank_pid")]
        public string BankPid { get; set; }

        /// <summary>
        /// 商户对应银行所开立的结算卡信息
        /// </summary>
        [XmlArray("bankcard_info")]
        [XmlArrayItem("bank_card_info")]
        public List<BankCardInfo> BankcardInfo { get; set; }

        /// <summary>
        /// 商户证件编号（企业或者个体工商户提供营业执照，事业单位提供事证号）
        /// </summary>
        [XmlElement("business_license")]
        public string BusinessLicense { get; set; }

        /// <summary>
        /// 商户证件类型，取值范围：NATIONAL_LEGAL：营业执照；NATIONAL_LEGAL_MERGE:营业执照(多证合一)；INST_RGST_CTF：事业单位法人证书
        /// </summary>
        [XmlElement("business_license_type")]
        public string BusinessLicenseType { get; set; }

        /// <summary>
        /// 商户联系人信息
        /// </summary>
        [XmlArray("contact_info")]
        [XmlArrayItem("contact_info")]
        public List<ContactInfo> ContactInfo { get; set; }

        /// <summary>
        /// 支付机构pid/source id；服务商PID；
        /// </summary>
        [XmlElement("isv_pid")]
        public string IsvPid { get; set; }

        /// <summary>
        /// 商户的支付宝账号
        /// </summary>
        [XmlArray("logon_id")]
        [XmlArrayItem("string")]
        public List<string> LogonId { get; set; }

        /// <summary>
        /// mcc编码
        /// </summary>
        [XmlElement("mcc")]
        public string Mcc { get; set; }

        /// <summary>
        /// 商户特殊资质等
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 商户的支付二维码中信息，用于营销活动
        /// </summary>
        [XmlArray("pay_code_info")]
        [XmlArrayItem("string")]
        public List<string> PayCodeInfo { get; set; }

        /// <summary>
        /// 商户客服电话
        /// </summary>
        [XmlElement("service_phone")]
        public string ServicePhone { get; set; }

        /// <summary>
        /// 商户在银行端的签约时间
        /// </summary>
        [XmlElement("sign_bank_time")]
        public string SignBankTime { get; set; }

        /// <summary>
        /// 站点信息
        /// </summary>
        [XmlArray("site_info")]
        [XmlArrayItem("site_info")]
        public List<SiteInfo> SiteInfo { get; set; }

        /// <summary>
        /// 商户在受理机构的唯一代码，该代号在该机构下保持唯一；extenalID；
        /// </summary>
        [XmlElement("unique_id_by_bank")]
        public string UniqueIdByBank { get; set; }

        /// <summary>
        /// 商户在支付机构的的唯一代码；服务商对该商户分配的ID；
        /// </summary>
        [XmlElement("unique_id_by_isv")]
        public string UniqueIdByIsv { get; set; }
    }
}
