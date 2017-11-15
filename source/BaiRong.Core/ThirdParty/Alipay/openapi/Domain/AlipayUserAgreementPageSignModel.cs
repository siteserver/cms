using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserAgreementPageSignModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserAgreementPageSignModel : AopObject
    {
        /// <summary>
        /// 请按当前接入的方式进行填充，且输入值必须为文档中的参数取值范围。  扫码或者短信页面签约需要拼装http的请求地址访问中间页面，钱包h5页面签约可直接拼接schema的请求地址
        /// </summary>
        [XmlElement("access_params")]
        public AccessParams AccessParams { get; set; }

        /// <summary>
        /// 商户签约号，代扣协议中标示用户的唯一签约号（确保在商户系统中唯一）。  格式规则：支持大写小写字母和数字，最长32位。  商户系统按需传入，如果同一用户在同一产品码、同一签约场景下，签订了多份代扣协议，那么需要指定并传入该值。
        /// </summary>
        [XmlElement("external_agreement_no")]
        public string ExternalAgreementNo { get; set; }

        /// <summary>
        /// 用户在商户网站的登录账号，用于在签约页面展示，如果为空，则不展示
        /// </summary>
        [XmlElement("external_logon_id")]
        public string ExternalLogonId { get; set; }

        /// <summary>
        /// 用户实名信息参数，包含：姓名、身份证号。商户传入用户实名信息参数，支付宝会对比用户在支付宝端的实名信息。
        /// </summary>
        [XmlElement("identity_params")]
        public IdentityParams IdentityParams { get; set; }

        /// <summary>
        /// 个人签约产品码，商户和支付宝签约时确定，商户可咨询技术支持。
        /// </summary>
        [XmlElement("personal_product_code")]
        public string PersonalProductCode { get; set; }

        /// <summary>
        /// 签约产品属性，json格式
        /// </summary>
        [XmlElement("prod_params")]
        public ProdParams ProdParams { get; set; }

        /// <summary>
        /// 销售产品码，商户签约的支付宝合同所对应的产品码。
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }

        /// <summary>
        /// 签约营销参数，此值为json格式；具体的key需与营销约定
        /// </summary>
        [XmlElement("promo_params")]
        public string PromoParams { get; set; }

        /// <summary>
        /// 协议签约场景，商户和支付宝签约时确定，商户可咨询技术支持。  当传入商户签约号external_agreement_no时，场景不能为默认值DEFAULT|DEFAULT。
        /// </summary>
        [XmlElement("sign_scene")]
        public string SignScene { get; set; }

        /// <summary>
        /// 当前用户签约请求的协议有效周期。  整形数字加上时间单位的协议有效期，从发起签约请求的时间开始算起。  目前支持的时间单位：  1. d：天  2. m：月  如果未传入，默认为长期有效。
        /// </summary>
        [XmlElement("sign_validity_period")]
        public string SignValidityPeriod { get; set; }

        /// <summary>
        /// 签约第三方主体类型。对于三方协议，表示当前用户和哪一类的第三方主体进行签约。  取值范围：  1. PARTNER（平台商户）  2. MERCHANT（集团商户），集团下子商户可共享用户签约内容  默认为PARTNER。
        /// </summary>
        [XmlElement("third_party_type")]
        public string ThirdPartyType { get; set; }

        /// <summary>
        /// 芝麻授权信息，针对于信用代扣签约。json格式。
        /// </summary>
        [XmlElement("zm_auth_params")]
        public ZmAuthParams ZmAuthParams { get; set; }
    }
}
