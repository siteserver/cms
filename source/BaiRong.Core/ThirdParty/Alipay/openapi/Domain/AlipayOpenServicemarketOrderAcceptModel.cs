using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenServicemarketOrderAcceptModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenServicemarketOrderAcceptModel : AopObject
    {
        /// <summary>
        /// 服务商品订单ID
        /// </summary>
        [XmlElement("commodity_order_id")]
        public string CommodityOrderId { get; set; }
    }
}
