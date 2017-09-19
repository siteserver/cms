using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// TicketDetailInfo Data Structure.
    /// </summary>
    [Serializable]
    public class TicketDetailInfo : AopObject
    {
        /// <summary>
        /// 总金额，元为单位
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 终点站编码
        /// </summary>
        [XmlElement("end_station")]
        public string EndStation { get; set; }

        /// <summary>
        /// 终点站中文名称
        /// </summary>
        [XmlElement("end_station_name")]
        public string EndStationName { get; set; }

        /// <summary>
        /// 票数量
        /// </summary>
        [XmlElement("quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// 起点站编码
        /// </summary>
        [XmlElement("start_station")]
        public string StartStation { get; set; }

        /// <summary>
        /// 起点站中文名称
        /// </summary>
        [XmlElement("start_station_name")]
        public string StartStationName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 单价，元为单位
        /// </summary>
        [XmlElement("ticket_price")]
        public string TicketPrice { get; set; }

        /// <summary>
        /// 票类型
        /// </summary>
        [XmlElement("ticket_type")]
        public string TicketType { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
