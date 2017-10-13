using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketShopCategoryQueryResponse.
    /// </summary>
    public class AlipayOfflineMarketShopCategoryQueryResponse : AopResponse
    {
        /// <summary>
        /// 门店类目配置信息，包括能够开店的叶子节点类目信息，以及类目约束配置信息。
        /// </summary>
        [XmlArray("shop_category_config_infos")]
        [XmlArrayItem("shop_category_config_info")]
        public List<ShopCategoryConfigInfo> ShopCategoryConfigInfos { get; set; }
    }
}
