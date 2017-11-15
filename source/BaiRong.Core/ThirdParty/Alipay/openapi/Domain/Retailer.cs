using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// Retailer Data Structure.
    /// </summary>
    [Serializable]
    public class Retailer : AopObject
    {
        /// <summary>
        /// 零售商名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 零售商pid
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }
    }
}
