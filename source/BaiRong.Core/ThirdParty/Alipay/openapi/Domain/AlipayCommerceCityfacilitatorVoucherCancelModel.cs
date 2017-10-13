using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceCityfacilitatorVoucherCancelModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceCityfacilitatorVoucherCancelModel : AopObject
    {
        /// <summary>
        /// 渠道商提供的其它信息
        /// </summary>
        [XmlElement("biz_info_ext")]
        public string BizInfoExt { get; set; }

        /// <summary>
        /// 城市标准码
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 商户退款时间
        /// </summary>
        [XmlElement("operate_time")]
        public string OperateTime { get; set; }

        /// <summary>
        /// 核销号
        /// </summary>
        [XmlElement("ticket_no")]
        public string TicketNo { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
