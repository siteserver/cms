using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenServicemarketCommodityShopOfflineModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenServicemarketCommodityShopOfflineModel : AopObject
    {
        /// <summary>
        /// 服务商户ID
        /// </summary>
        [XmlElement("commodity_id")]
        public string CommodityId { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}
