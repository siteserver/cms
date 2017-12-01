using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenServicemarketCommodityShopOnlineModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenServicemarketCommodityShopOnlineModel : AopObject
    {
        /// <summary>
        /// 服务插件ID
        /// </summary>
        [XmlElement("commodity_id")]
        public string CommodityId { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}
