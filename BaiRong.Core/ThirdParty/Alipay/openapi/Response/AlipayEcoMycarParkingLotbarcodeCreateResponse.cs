using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoMycarParkingLotbarcodeCreateResponse.
    /// </summary>
    public class AlipayEcoMycarParkingLotbarcodeCreateResponse : AopResponse
    {
        /// <summary>
        /// 返回二维码链接地址
        /// </summary>
        [XmlElement("qrcode_url")]
        public string QrcodeUrl { get; set; }

        /// <summary>
        /// 返回状态：1为成功，0为失败
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
