using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipaySecurityProdFingerprintApplyResponse.
    /// </summary>
    public class AlipaySecurityProdFingerprintApplyResponse : AopResponse
    {
        /// <summary>
        /// IFAA标准中的校验类型，目前1为指纹
        /// </summary>
        [XmlElement("auth_type")]
        public string AuthType { get; set; }

        /// <summary>
        /// 设备的唯一ID，IFAA标准体系中的设备的唯一标识，用于关联设备的开通状态
        /// </summary>
        [XmlElement("device_id")]
        public string DeviceId { get; set; }

        /// <summary>
        /// IFAA标准中用于关联IFAA Server和业务方Server开通状态的token，此token用于后续校验和注销操作。
        /// </summary>
        [XmlElement("token")]
        public string Token { get; set; }
    }
}
