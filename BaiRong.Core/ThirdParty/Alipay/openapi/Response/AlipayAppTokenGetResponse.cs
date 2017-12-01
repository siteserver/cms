using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayAppTokenGetResponse.
    /// </summary>
    public class AlipayAppTokenGetResponse : AopResponse
    {
        /// <summary>
        /// 应用访问令牌
        /// </summary>
        [XmlElement("app_access_token")]
        public string AppAccessToken { get; set; }

        /// <summary>
        /// 应用访问凭证有效时间，单位：秒
        /// </summary>
        [XmlElement("expires_in")]
        public long ExpiresIn { get; set; }
    }
}
