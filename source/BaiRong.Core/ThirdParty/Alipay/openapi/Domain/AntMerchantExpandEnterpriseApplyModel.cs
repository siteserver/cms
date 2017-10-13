using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntMerchantExpandEnterpriseApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntMerchantExpandEnterpriseApplyModel : AopObject
    {
        /// <summary>
        /// 企业基础信息
        /// </summary>
        [XmlElement("base_info")]
        public BaseInfo BaseInfo { get; set; }

        /// <summary>
        /// 企业对公账户信息
        /// </summary>
        [XmlElement("business_bank_account_info")]
        public BusinessBankAccountInfo BusinessBankAccountInfo { get; set; }

        /// <summary>
        /// 企业营业执照信息
        /// </summary>
        [XmlElement("business_license_info")]
        public BusinessLicenceInfo BusinessLicenseInfo { get; set; }

        /// <summary>
        /// 企业级商户法人信息
        /// </summary>
        [XmlElement("legal_representative_info")]
        public LegalRepresentativeInfo LegalRepresentativeInfo { get; set; }

        /// <summary>
        /// 支付宝登录别名,必须是邮箱地址。入驻申请结果会通知到该邮箱地址或手机号码。如填入的是已有的企业版支付宝账号则后续认证与签约基于该账号进行，如填入的邮箱地址没有对应的支付宝账号则用该邮箱地址创建一个企业版支付宝账户，如填入的是已有的非企业版支付宝账号则预校验失败。
        /// </summary>
        [XmlElement("login_id")]
        public string LoginId { get; set; }

        /// <summary>
        /// 外部入驻申请单据号，需保证在开发者端不重复
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 企业的门店信息，签约当面付时必选
        /// </summary>
        [XmlElement("shop_info")]
        public ShopInfo ShopInfo { get; set; }
    }
}
