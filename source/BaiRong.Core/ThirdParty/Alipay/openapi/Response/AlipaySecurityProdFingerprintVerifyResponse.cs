using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipaySecurityProdFingerprintVerifyResponse.
    /// </summary>
    public class AlipaySecurityProdFingerprintVerifyResponse : AopResponse
    {
        /// <summary>
        /// IFAA服务端校验结果，true为通过，false为未通过
        /// </summary>
        [XmlElement("auth_result")]
        public bool AuthResult { get; set; }

        /// <summary>
        /// IFAA标准中用于关联IFAA Server和业务方Server开通状态的token，此token用于提供给业务方关联校验结果。
        /// </summary>
        [XmlElement("token")]
        public string Token { get; set; }
    }
}
