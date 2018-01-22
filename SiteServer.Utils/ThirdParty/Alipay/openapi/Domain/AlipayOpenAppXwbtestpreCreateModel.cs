using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenAppXwbtestpreCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenAppXwbtestpreCreateModel : AopObject
    {
        /// <summary>
        /// 1
        /// </summary>
        [XmlElement("sd")]
        public string Sd { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        [XmlElement("xwb")]
        public string Xwb { get; set; }
    }
}
