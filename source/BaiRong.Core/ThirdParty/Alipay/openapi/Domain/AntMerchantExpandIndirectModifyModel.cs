using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntMerchantExpandIndirectModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntMerchantExpandIndirectModifyModel : AopObject
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
        /// 商户经营类目，参考文档：https://doc.open.alipay.com/doc2/detail?&docType=1&articleId=105444
        /// </summary>
        [XmlElement("category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 商户负责人信息
        /// </summary>
        [XmlArray("contact_info")]
        [XmlArrayItem("contact_info")]
        public List<ContactInfo> ContactInfo { get; set; }

        /// <summary>
        /// 商户编号，由机构定义，需要保证在机构下唯一，与sub_merchant_id二选一
        /// </summary>
        [XmlElement("external_id")]
        public string ExternalId { get; set; }

        /// <summary>
        /// 受理商户的支付宝账号（用于关联商户生活号、原服务窗，打通口碑营销活动）
        /// </summary>
        [XmlArray("logon_id")]
        [XmlArrayItem("string")]
        public List<string> LogonId { get; set; }

        /// <summary>
        /// 商户备注信息，可填写额外信息
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 受理商户的固定二维码链接地址（即一码多付页面地址，用于后续支付宝营销活动）  商户所属的银行或ISV 给商户的二维码url值  一码多付方案:https://doc.open.alipay.com/docs/doc.htm?&docType=1&articleId=105672
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
        /// 商户来源机构标识，填写机构在支付宝的pid
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }

        /// <summary>
        /// 商户在支付宝入驻成功后，生成的支付宝内全局唯一的商户编号，与external_id二选一
        /// </summary>
        [XmlElement("sub_merchant_id")]
        public string SubMerchantId { get; set; }
    }
}
