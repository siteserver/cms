using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayCommerceCityfacilitatorVoucherGenerateResponse.
    /// </summary>
    public class AlipayCommerceCityfacilitatorVoucherGenerateResponse : AopResponse
    {
        /// <summary>
        /// 核销码过期时间
        /// </summary>
        [XmlElement("expired_date")]
        public string ExpiredDate { get; set; }

        /// <summary>
        /// 地铁购票二维码编码，可自定义
        /// </summary>
        [XmlElement("qr_code_no")]
        public string QrCodeNo { get; set; }

        /// <summary>
        /// 地铁购票的核销码
        /// </summary>
        [XmlElement("ticket_no")]
        public string TicketNo { get; set; }
    }
}
