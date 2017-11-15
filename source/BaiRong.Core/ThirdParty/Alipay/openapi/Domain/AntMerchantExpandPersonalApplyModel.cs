using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntMerchantExpandPersonalApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntMerchantExpandPersonalApplyModel : AopObject
    {
        /// <summary>
        /// 企业基本信息
        /// </summary>
        [XmlElement("base_info")]
        public BaseInfo BaseInfo { get; set; }

        /// <summary>
        /// 营业执照信息
        /// </summary>
        [XmlElement("business_license_info")]
        public BusinessLicenceInfo BusinessLicenseInfo { get; set; }

        /// <summary>
        /// 支付宝登录别名,邮箱地址或手机号码，入驻申请结果会通知到该邮箱地址或手机号码。如填入的是已有的企业版支付宝账号则后续认证与签约基于该账号进行，如填入的邮箱地址或手机号码没有对应的支付宝账号则用该邮箱地址或手机号码创建一个企业版支付宝账户，如填入的是已有的非企业版支付宝账号则预校验失败。
        /// </summary>
        [XmlElement("login_id")]
        public string LoginId { get; set; }

        /// <summary>
        /// 个体工商户经营者信息
        /// </summary>
        [XmlElement("operator_info")]
        public OperatorInfo OperatorInfo { get; set; }

        /// <summary>
        /// 外部入驻申请单据号，需保证在开发者端不重复
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 工商个体户或个人银行账户信息
        /// </summary>
        [XmlElement("personal_bank_account_info")]
        public PersonnalBankAccountInfo PersonalBankAccountInfo { get; set; }

        /// <summary>
        /// 门店信息
        /// </summary>
        [XmlElement("shop_info")]
        public ShopInfo ShopInfo { get; set; }
    }
}
