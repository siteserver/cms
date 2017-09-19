using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicGisQueryResponse.
    /// </summary>
    public class AlipayOpenPublicGisQueryResponse : AopResponse
    {
        /// <summary>
        /// 精确度
        /// </summary>
        [XmlElement("accuracy")]
        public string Accuracy { get; set; }

        /// <summary>
        /// 经纬度所在位置
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }

        /// <summary>
        /// 纬度信息
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度信息
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 经纬度对应位置所在的省份
        /// </summary>
        [XmlElement("province")]
        public string Province { get; set; }
    }
}
