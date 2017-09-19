using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenAuthIndustryPlatformCreateTokenResponse.
    /// </summary>
    public class AlipayOpenAuthIndustryPlatformCreateTokenResponse : AopResponse
    {
        /// <summary>
        /// 授权码
        /// </summary>
        [XmlElement("auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// appid
        /// </summary>
        [XmlElement("requst_app_id")]
        public string RequstAppId { get; set; }

        /// <summary>
        /// scope
        /// </summary>
        [XmlElement("scope")]
        public string Scope { get; set; }
    }
}
