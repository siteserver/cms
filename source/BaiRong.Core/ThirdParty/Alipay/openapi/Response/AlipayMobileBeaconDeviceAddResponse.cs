using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMobileBeaconDeviceAddResponse.
    /// </summary>
    public class AlipayMobileBeaconDeviceAddResponse : AopResponse
    {
        /// <summary>
        /// 请求操作成功与否，200为成功
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 请求的处理结果
        /// </summary>
        [XmlElement("msg")]
        public string Msg { get; set; }
    }
}
