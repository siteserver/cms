using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketShopDiscountQueryResponse.
    /// </summary>
    public class AlipayOfflineMarketShopDiscountQueryResponse : AopResponse
    {
        /// <summary>
        /// 返回商户正在进行中的活动摘要列表数据，注意：如果入参query_type不传递或者传递错误值，该参数将不会返回。
        /// </summary>
        [XmlArray("camp_list")]
        [XmlArrayItem("camp_detail_info")]
        public List<CampDetailInfo> CampList { get; set; }

        /// <summary>
        /// 返回商户正在进行中的活动记录数，注意：如果入参query_type不传递或者传递错误值，该参数将不会返回。
        /// </summary>
        [XmlElement("camp_num")]
        public long CampNum { get; set; }

        /// <summary>
        /// 优惠信息列表，最大20条，按生效时间排序，注意：如果入参query_type传递了指定的值，该参数将不会返回。只有当query_type不传递时方可返回数据
        /// </summary>
        [XmlArray("discount_list")]
        [XmlArrayItem("shop_discount_info")]
        public List<ShopDiscountInfo> DiscountList { get; set; }

        /// <summary>
        /// 商品列表，最大20条，按申领数量排序，注意：如果入参query_type传递了指定的值，该参数将不会返回。只有当query_type不传递时方可返回数据
        /// </summary>
        [XmlArray("item_list")]
        [XmlArrayItem("shop_discount_info")]
        public List<ShopDiscountInfo> ItemList { get; set; }
    }
}
