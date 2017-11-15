using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMobileBeaconDeviceModifyResponse.
    /// </summary>
    public class AlipayMobileBeaconDeviceModifyResponse : AopResponse
    {
        /// <summary>
        /// 返回的操作码
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 操作结果说明
        /// </summary>
        [XmlElement("msg")]
        public string Msg { get; set; }
    }
}
