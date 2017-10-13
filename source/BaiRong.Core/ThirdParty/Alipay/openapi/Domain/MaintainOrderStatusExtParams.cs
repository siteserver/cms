using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MaintainOrderStatusExtParams Data Structure.
    /// </summary>
    [Serializable]
    public class MaintainOrderStatusExtParams : AopObject
    {
        /// <summary>
        /// 编码
        /// </summary>
        [XmlElement("logistics_code")]
        public string LogisticsCode { get; set; }

        /// <summary>
        /// 快递发货商
        /// </summary>
        [XmlElement("logistics_company")]
        public string LogisticsCompany { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        [XmlElement("logistics_no")]
        public string LogisticsNo { get; set; }

        /// <summary>
        /// 4S订单状态
        /// </summary>
        [XmlElement("order_status_txt")]
        public string OrderStatusTxt { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        [XmlElement("receiver_addr")]
        public string ReceiverAddr { get; set; }

        /// <summary>
        /// 发货地址
        /// </summary>
        [XmlElement("sender_addr")]
        public string SenderAddr { get; set; }
    }
}
