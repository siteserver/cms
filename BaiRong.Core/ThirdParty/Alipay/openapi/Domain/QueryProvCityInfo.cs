using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// QueryProvCityInfo Data Structure.
    /// </summary>
    [Serializable]
    public class QueryProvCityInfo : AopObject
    {
        /// <summary>
        /// 省市编号
        /// </summary>
        [XmlElement("adcode")]
        public string Adcode { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
