using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingIbsInfo Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingIbsInfo : AopObject
    {
        /// <summary>
        /// 精度
        /// </summary>
        [XmlElement("accuracy")]
        public string Accuracy { get; set; }

        /// <summary>
        /// 海拔
        /// </summary>
        [XmlElement("altitude")]
        public string Altitude { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 时间ms
        /// </summary>
        [XmlElement("time")]
        public string Time { get; set; }
    }
}
