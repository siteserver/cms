using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// StationDetailInfo Data Structure.
    /// </summary>
    [Serializable]
    public class StationDetailInfo : AopObject
    {
        /// <summary>
        /// 站点编码
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 站点外部编码
        /// </summary>
        [XmlElement("ext_code")]
        public string ExtCode { get; set; }

        /// <summary>
        /// 站点中文名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
