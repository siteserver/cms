using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarViolationCityPushModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarViolationCityPushModel : AopObject
    {
        /// <summary>
        /// 城市编码
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 该城市规则是新增还是更新, 0:新增,1:更新
        /// </summary>
        [XmlElement("push_type")]
        public string PushType { get; set; }

        /// <summary>
        /// 该城市是否支持违章查询,0:支持,1:不支持
        /// </summary>
        [XmlElement("service_status")]
        public string ServiceStatus { get; set; }
    }
}
