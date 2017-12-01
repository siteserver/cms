using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiRetailShopitemUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiRetailShopitemUploadModel : AopObject
    {
        /// <summary>
        /// 上传的商品集合
        /// </summary>
        [XmlArray("shop_items")]
        [XmlArrayItem("request_ext_shop_item")]
        public List<RequestExtShopItem> ShopItems { get; set; }
    }
}
