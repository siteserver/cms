using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceCityfacilitatorVoucherConfirmModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceCityfacilitatorVoucherConfirmModel : AopObject
    {
        /// <summary>
        /// 金额，元为单位
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 渠道商提供的其它信息
        /// </summary>
        [XmlElement("biz_info_ext")]
        public string BizInfoExt { get; set; }

        /// <summary>
        /// 该笔请求的唯一编号，有值请求强校验
        /// </summary>
        [XmlElement("biz_request_id")]
        public string BizRequestId { get; set; }

        /// <summary>
        /// 城市标准码
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 终点站编号
        /// </summary>
        [XmlElement("end_station")]
        public string EndStation { get; set; }

        /// <summary>
        /// 单张票编号列表，多个逗号分隔
        /// </summary>
        [XmlElement("exchange_ids")]
        public string ExchangeIds { get; set; }

        /// <summary>
        /// 商户核销时间
        /// </summary>
        [XmlElement("operate_time")]
        public string OperateTime { get; set; }

        /// <summary>
        /// 商户的交易号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 票数
        /// </summary>
        [XmlElement("quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// 请求方标识
        /// </summary>
        [XmlElement("seller_id")]
        public string SellerId { get; set; }

        /// <summary>
        /// 起点站编号
        /// </summary>
        [XmlElement("start_station")]
        public string StartStation { get; set; }

        /// <summary>
        /// 核销号
        /// </summary>
        [XmlElement("ticket_no")]
        public string TicketNo { get; set; }

        /// <summary>
        /// 票单价，元为单位
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
