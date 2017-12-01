using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPlatformUseridGetResponse.
    /// </summary>
    public class AlipayPlatformUseridGetResponse : AopResponse
    {
        /// <summary>
        /// id字典，key为openId，value为userId
        /// </summary>
        [XmlElement("dict")]
        public string Dict { get; set; }
    }
}
