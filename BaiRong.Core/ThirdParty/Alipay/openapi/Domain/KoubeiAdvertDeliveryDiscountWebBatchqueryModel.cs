using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertDeliveryDiscountWebBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertDeliveryDiscountWebBatchqueryModel : AopObject
    {
        /// <summary>
        /// 分配的固定的渠道CODE，需要找运营申请
        /// </summary>
        [XmlElement("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// 纬度，根据经纬度查询附近的券
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度，根据经纬度查询附近的券
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 手机号码，根据手机号码查询支付宝账户ID
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 门店ID，如果设置门店，则查询门店下发行的券
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}
