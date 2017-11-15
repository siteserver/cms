using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipaySecurityProdFingerprintVerifyInitializeResponse.
    /// </summary>
    public class AlipaySecurityProdFingerprintVerifyInitializeResponse : AopResponse
    {
        /// <summary>
        /// ifaf_message:校验阶段服务端返回的协议体数据，对应《IFAA本地免密技术规范》中的IFAFMessage，内容中包含服务端的校验数据。
        /// </summary>
        [XmlElement("server_response")]
        public string ServerResponse { get; set; }
    }
}
