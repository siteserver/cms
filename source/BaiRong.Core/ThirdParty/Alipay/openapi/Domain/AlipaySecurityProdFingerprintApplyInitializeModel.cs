using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityProdFingerprintApplyInitializeModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityProdFingerprintApplyInitializeModel : AopObject
    {
        /// <summary>
        /// IFAA标准中的校验类型，目前1为指纹
        /// </summary>
        [XmlElement("auth_type")]
        public string AuthType { get; set; }

        /// <summary>
        /// IFAA协议的版本，目前为2.0
        /// </summary>
        [XmlElement("ifaa_version")]
        public string IfaaVersion { get; set; }

        /// <summary>
        /// IFAA协议客户端静态信息，调用IFAA客户端SDK接口获取secData，透传至本参数
        /// </summary>
        [XmlElement("sec_data")]
        public string SecData { get; set; }
    }
}
