using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoMycarParkingParkinglotinfoCreateResponse.
    /// </summary>
    public class AlipayEcoMycarParkingParkinglotinfoCreateResponse : AopResponse
    {
        /// <summary>
        /// 支付宝返回停车场id。成功不为空，失败返回空
        /// </summary>
        [XmlElement("parking_id")]
        public string ParkingId { get; set; }
    }
}
