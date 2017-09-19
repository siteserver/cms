using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OrderLogisticsInfo Data Structure.
    /// </summary>
    [Serializable]
    public class OrderLogisticsInfo : AopObject
    {
        /// <summary>
        /// 消费者下单线下服务时，填写的服务地址
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [XmlElement("contact_name")]
        public string ContactName { get; set; }

        /// <summary>
        /// 订单的物流扩展信息，包括服务开始时间、服务结束时间
        /// </summary>
        [XmlElement("ext_info")]
        public OrderLogisticsExtInfo ExtInfo { get; set; }

        /// <summary>
        /// 消费者地址纬度（高德坐标系）
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 消费者地址经度（高德坐标系）
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 商家电话绑定的消费者手机号（阿里小号）
        /// </summary>
        [XmlElement("merchant_bind_mobile")]
        public string MerchantBindMobile { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }
    }
}
