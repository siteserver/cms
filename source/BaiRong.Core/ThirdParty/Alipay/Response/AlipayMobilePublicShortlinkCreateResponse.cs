using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMobilePublicShortlinkCreateResponse.
    /// </summary>
    public class AlipayMobilePublicShortlinkCreateResponse : AopResponse
    {
        /// <summary>
        /// 结果码
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        [XmlElement("msg")]
        public string Msg { get; set; }

        /// <summary>
        /// 短链接url
        /// </summary>
        [XmlElement("shortlink")]
        public string Shortlink { get; set; }
    }
}
