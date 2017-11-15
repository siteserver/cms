using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// RequestExtShopItem Data Structure.
    /// </summary>
    [Serializable]
    public class RequestExtShopItem : AopObject
    {
        /// <summary>
        /// 店铺商品的品牌名称
        /// </summary>
        [XmlElement("brand_code")]
        public string BrandCode { get; set; }

        /// <summary>
        /// 店铺商品的商品类别
        /// </summary>
        [XmlElement("category_code")]
        public string CategoryCode { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// 店铺商品SKU
        /// </summary>
        [XmlElement("item_code")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 口碑门店id
        /// </summary>
        [XmlElement("kb_shop_id")]
        public string KbShopId { get; set; }

        /// <summary>
        /// 商品参考价格
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 店铺商品的名称
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}
