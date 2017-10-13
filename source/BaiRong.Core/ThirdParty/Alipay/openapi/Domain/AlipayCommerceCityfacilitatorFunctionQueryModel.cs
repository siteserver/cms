using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceCityfacilitatorFunctionQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceCityfacilitatorFunctionQueryModel : AopObject
    {
        /// <summary>
        /// 城市国家标准编码
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 终端设备编码，android可直接获取设备的devicecode值
        /// </summary>
        [XmlElement("device_code")]
        public string DeviceCode { get; set; }
    }
}
