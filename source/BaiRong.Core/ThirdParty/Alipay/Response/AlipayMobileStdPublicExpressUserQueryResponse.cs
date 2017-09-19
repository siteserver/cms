using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMobileStdPublicExpressUserQueryResponse.
    /// </summary>
    public class AlipayMobileStdPublicExpressUserQueryResponse : AopResponse
    {
        /// <summary>
        /// 证件号
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 证件类型，身份证
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }
    }
}
