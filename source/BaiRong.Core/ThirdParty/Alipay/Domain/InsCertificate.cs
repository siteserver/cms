using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsCertificate Data Structure.
    /// </summary>
    [Serializable]
    public class InsCertificate : AopObject
    {
        /// <summary>
        /// 发奖凭证ID
        /// </summary>
        [XmlElement("certificate_id")]
        public string CertificateId { get; set; }

        /// <summary>
        /// 发奖凭证类型;GIFT_INSURANCE:赠险
        /// </summary>
        [XmlElement("certificate_type")]
        public string CertificateType { get; set; }

        /// <summary>
        /// 发奖凭证值
        /// </summary>
        [XmlElement("certificate_value")]
        public string CertificateValue { get; set; }
    }
}
