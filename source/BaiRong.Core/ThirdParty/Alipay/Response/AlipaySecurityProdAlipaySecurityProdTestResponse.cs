using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipaySecurityProdAlipaySecurityProdTestResponse.
    /// </summary>
    public class AlipaySecurityProdAlipaySecurityProdTestResponse : AopResponse
    {
        /// <summary>
        /// ddd
        /// </summary>
        [XmlElement("admin")]
        public string Admin { get; set; }
    }
}
