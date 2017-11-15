using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenAppCodetesttestResponse.
    /// </summary>
    public class AlipayOpenAppCodetesttestResponse : AopResponse
    {
        /// <summary>
        /// 测试测试测试
        /// </summary>
        [XmlElement("testtesttest")]
        public string Testtesttest { get; set; }
    }
}
