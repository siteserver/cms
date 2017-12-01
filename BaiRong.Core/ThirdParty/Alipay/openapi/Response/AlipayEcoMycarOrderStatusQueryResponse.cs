using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoMycarOrderStatusQueryResponse.
    /// </summary>
    public class AlipayEcoMycarOrderStatusQueryResponse : AopResponse
    {
        /// <summary>
        /// 支付宝交易流水号订单
        /// </summary>
        [XmlElement("alipay_order_id")]
        public string AlipayOrderId { get; set; }

        /// <summary>
        /// 车平台订单
        /// </summary>
        [XmlElement("car_order_id")]
        public string CarOrderId { get; set; }

        /// <summary>
        /// 设备商订单id
        /// </summary>
        [XmlElement("equipment_order_id")]
        public string EquipmentOrderId { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [XmlElement("pay_money")]
        public string PayMoney { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        [XmlElement("pay_status")]
        public string PayStatus { get; set; }

        /// <summary>
        /// 支付的时间，格式"yyyy-MM-ddHH:mm:ss"
        /// </summary>
        [XmlElement("pay_time")]
        public string PayTime { get; set; }

        /// <summary>
        /// 支付方式（1为支付宝在线缴费，2为支付宝代扣缴费）
        /// </summary>
        [XmlElement("pay_type")]
        public string PayType { get; set; }

        /// <summary>
        /// 返回状态  1为成功  0为失败
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
