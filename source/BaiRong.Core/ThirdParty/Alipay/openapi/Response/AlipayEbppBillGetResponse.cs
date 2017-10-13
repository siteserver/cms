using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEbppBillGetResponse.
    /// </summary>
    public class AlipayEbppBillGetResponse : AopResponse
    {
        /// <summary>
        /// 支付宝的业务订单号，具有唯一性。
        /// </summary>
        [XmlElement("alipay_order_no")]
        public string AlipayOrderNo { get; set; }

        /// <summary>
        /// 账单的账期，例如201203表示2012年3月的账单。
        /// </summary>
        [XmlElement("bill_date")]
        public string BillDate { get; set; }

        /// <summary>
        /// 账单单据号，例如水费单号，手机号，电费号，信用卡卡号。没有唯一性要求。
        /// </summary>
        [XmlElement("bill_key")]
        public string BillKey { get; set; }

        /// <summary>
        /// 支付宝给每个出账机构指定了一个对应的英文短名称来唯一表示该收费单位。
        /// </summary>
        [XmlElement("charge_inst")]
        public string ChargeInst { get; set; }

        /// <summary>
        /// 出账机构中文名称。
        /// </summary>
        [XmlElement("charge_inst_name")]
        public string ChargeInstName { get; set; }

        /// <summary>
        /// 输出机构的业务流水号，需要保证唯一性。
        /// </summary>
        [XmlElement("merchant_order_no")]
        public string MerchantOrderNo { get; set; }

        /// <summary>
        /// 账单的状态。 INIT:等待付款，SUCCESS:成功,FAILED:失败。
        /// </summary>
        [XmlElement("order_status")]
        public string OrderStatus { get; set; }

        /// <summary>
        /// 支付宝订单类型。公共事业缴纳JF,信用卡还款HK
        /// </summary>
        [XmlElement("order_type")]
        public string OrderType { get; set; }

        /// <summary>
        /// 拥有该账单的用户姓名
        /// </summary>
        [XmlElement("owner_name")]
        public string OwnerName { get; set; }

        /// <summary>
        /// 缴费金额。用户支付的总金额。单位为：RMB Yuan。取值范围为[0.01，100000000.00]，精确到小数点后两位。
        /// </summary>
        [XmlElement("pay_amount")]
        public string PayAmount { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        [XmlElement("pay_time")]
        public string PayTime { get; set; }

        /// <summary>
        /// 账单的服务费
        /// </summary>
        [XmlElement("service_amount")]
        public string ServiceAmount { get; set; }

        /// <summary>
        /// 子业务类型是业务类型的下一级概念，例如：WATER表示JF下面的水费，ELECTRIC表示JF下面的电费，GAS表示JF下面的燃气费。
        /// </summary>
        [XmlElement("sub_order_type")]
        public string SubOrderType { get; set; }

        /// <summary>
        /// 交通违章地点，sub_order_type=TRAFFIC时有值
        /// </summary>
        [XmlElement("traffic_location")]
        public string TrafficLocation { get; set; }

        /// <summary>
        /// 违章行为，sub_order_type=TRAFFIC时有值。
        /// </summary>
        [XmlElement("traffic_regulations")]
        public string TrafficRegulations { get; set; }
    }
}
