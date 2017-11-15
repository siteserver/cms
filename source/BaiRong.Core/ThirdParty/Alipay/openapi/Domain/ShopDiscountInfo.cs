using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ShopDiscountInfo Data Structure.
    /// </summary>
    [Serializable]
    public class ShopDiscountInfo : AopObject
    {
        /// <summary>
        /// 图片url
        /// </summary>
        [XmlElement("cover")]
        public string Cover { get; set; }

        /// <summary>
        /// 优惠信息描述
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// 是否全场。  全场：Y，单品：N
        /// </summary>
        [XmlElement("is_all")]
        public string IsAll { get; set; }

        /// <summary>
        /// 支付宝商品id
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 子类型
        /// </summary>
        [XmlElement("promo_sub_type")]
        public string PromoSubType { get; set; }

        /// <summary>
        /// 优惠类型。  优惠：discount  商品：item
        /// </summary>
        [XmlElement("promotion_type")]
        public string PromotionType { get; set; }

        /// <summary>
        /// 领用模式。自动领用：AUTO_OBTAIN。手动领用：OBTAIN
        /// </summary>
        [XmlElement("purchase_mode")]
        public string PurchaseMode { get; set; }

        /// <summary>
        /// 已用数量
        /// </summary>
        [XmlElement("sales_quantity")]
        public string SalesQuantity { get; set; }

        /// <summary>
        /// 优惠信息标题
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }
    }
}
