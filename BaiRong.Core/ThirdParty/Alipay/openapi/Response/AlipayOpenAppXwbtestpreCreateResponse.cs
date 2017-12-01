using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenAppXwbtestpreCreateResponse.
    /// </summary>
    public class AlipayOpenAppXwbtestpreCreateResponse : AopResponse
    {
        /// <summary>
        /// s
        /// </summary>
        [XmlElement("forestuser")]
        public bool Forestuser { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        [XmlElement("sd")]
        public string Sd { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        [XmlElement("sdd")]
        public string Sdd { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        [XmlElement("sdf")]
        public string Sdf { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [XmlElement("sdfsdf")]
        public bool Sdfsdf { get; set; }
    }
}
