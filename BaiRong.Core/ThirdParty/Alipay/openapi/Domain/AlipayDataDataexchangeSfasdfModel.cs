using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataDataexchangeSfasdfModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataDataexchangeSfasdfModel : AopObject
    {
        /// <summary>
        /// sdafsdfsaf
        /// </summary>
        [XmlElement("adsfghjf")]
        public AlipayItemVoucherTemplete Adsfghjf { get; set; }

        /// <summary>
        /// ghjffdssfghj
        /// </summary>
        [XmlArray("easadasfd")]
        [XmlArrayItem("string")]
        public List<string> Easadasfd { get; set; }

        /// <summary>
        /// dsfghdsagfhd
        /// </summary>
        [XmlArray("gdfsa")]
        [XmlArrayItem("string")]
        public List<string> Gdfsa { get; set; }

        /// <summary>
        /// ghjkhg
        /// </summary>
        [XmlElement("hjgdfs")]
        public string Hjgdfs { get; set; }

        /// <summary>
        /// sdgfjhkg
        /// </summary>
        [XmlArray("sdfgsdfg")]
        [XmlArrayItem("string")]
        public List<string> Sdfgsdfg { get; set; }

        /// <summary>
        /// ASGFDGASaaf
        /// </summary>
        [XmlArray("wehtegf")]
        [XmlArrayItem("string")]
        public List<string> Wehtegf { get; set; }
    }
}
