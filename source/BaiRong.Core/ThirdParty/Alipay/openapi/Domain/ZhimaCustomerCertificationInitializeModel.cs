using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZhimaCustomerCertificationInitializeModel Data Structure.
    /// </summary>
    [Serializable]
    public class ZhimaCustomerCertificationInitializeModel : AopObject
    {
        /// <summary>
        /// 认证场景码,常用的场景码有:  FACE:人脸认证      签约的协议决定了可以使用那些场景
        /// </summary>
        [XmlElement("biz_code")]
        public string BizCode { get; set; }

        /// <summary>
        /// 扩展业务参数,暂时没有用到,接口预留
        /// </summary>
        [XmlElement("ext_biz_param")]
        public string ExtBizParam { get; set; }

        /// <summary>
        /// 值为一个json串,必须包含身份类型identity_type,不同的身份类型需要的身份信息不同  当前支持:  身份信息为证件信息identity_type=CERT_INFO:  证件类型为身份证cert_type=IDENTITY_CARD,必要信息cert_name和cert_no    可以选填商户的用户主体principal_id,对应用户在商户端唯一标识,如果商户传了principal_id,后续会为商户提供更强大功能
        /// </summary>
        [XmlElement("identity_param")]
        public string IdentityParam { get; set; }

        /// <summary>
        /// 芝麻认证产品码,示例值为真实的产品码
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }

        /// <summary>
        /// 商户请求的唯一标志，32位长度的字母数字下划线组合。该标识作为对账的关键信息，商户要保证其唯一性.建议:前面几位字符是商户自定义的简称,中间可以使用一段日期,结尾可以使用一个序列
        /// </summary>
        [XmlElement("transaction_id")]
        public string TransactionId { get; set; }
    }
}
