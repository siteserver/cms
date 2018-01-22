using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SimpleMockModel Data Structure.
    /// </summary>
    [Serializable]
    public class SimpleMockModel : AopObject
    {
        /// <summary>
        /// 11
        /// </summary>
        [XmlElement("count_items")]
        public long CountItems { get; set; }

        /// <summary>
        /// 111
        /// </summary>
        [XmlElement("happen_time")]
        public string HappenTime { get; set; }

        /// <summary>
        /// 1.2f
        /// </summary>
        [XmlElement("price_num")]
        public string PriceNum { get; set; }

        /// <summary>
        /// false
        /// </summary>
        [XmlElement("right")]
        public bool Right { get; set; }

        /// <summary>
        /// trade_no
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
