using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiRetailShopitemBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiRetailShopitemBatchqueryModel : AopObject
    {
        /// <summary>
        /// 查询店铺商品查询入参
        /// </summary>
        [XmlArray("shop_items")]
        [XmlArrayItem("request_ext_shop_item_query")]
        public List<RequestExtShopItemQuery> ShopItems { get; set; }
    }
}
