using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayTrustUserAuthSendResponse.
    /// </summary>
    public class AlipayTrustUserAuthSendResponse : AopResponse
    {
        /// <summary>
        /// 当授权通知发送成功时，为T；否则用业务错误码表示
        /// </summary>
        [XmlElement("result")]
        public string Result { get; set; }
    }
}
