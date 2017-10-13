using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenAppPackagetestModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenAppPackagetestModel : AopObject
    {
        /// <summary>
        /// testtest
        /// </summary>
        [XmlElement("testparam")]
        public string Testparam { get; set; }

        /// <summary>
        /// testtest
        /// </summary>
        [XmlElement("testtest")]
        public string Testtest { get; set; }
    }
}
