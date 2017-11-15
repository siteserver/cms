using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoMycarParkingVehicleQueryResponse.
    /// </summary>
    public class AlipayEcoMycarParkingVehicleQueryResponse : AopResponse
    {
        /// <summary>
        /// 车牌信息（utf-8编码）
        /// </summary>
        [XmlElement("car_number")]
        public string CarNumber { get; set; }
    }
}
