using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DiscountInfo Data Structure.
    /// </summary>
    [Serializable]
    public class DiscountInfo : AopObject
    {
        /// <summary>
        /// 全场代金的门槛金额
        /// </summary>
        [XmlElement("apply_condition")]
        public string ApplyCondition { get; set; }

        /// <summary>
        /// 买M送N的描述
        /// </summary>
        [XmlElement("buy_send_desc")]
        public string BuySendDesc { get; set; }

        /// <summary>
        /// 折扣率  仅当券类型为折扣券时有效  有效折扣率取值范围0.11-0.99  仅允许保留小数点后两位
        /// </summary>
        [XmlElement("discount")]
        public string Discount { get; set; }

        /// <summary>
        /// 最近店铺离当前用户的距离
        /// </summary>
        [XmlElement("distance")]
        public string Distance { get; set; }

        /// <summary>
        /// 优惠结束时间
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 券的图片地址
        /// </summary>
        [XmlElement("image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// 优惠id
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 券的名称
        /// </summary>
        [XmlElement("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// 商品的一些标签
        /// </summary>
        [XmlElement("label")]
        public string Label { get; set; }

        /// <summary>
        /// 减至券的原价
        /// </summary>
        [XmlElement("original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// 每满thresholdPrice元减perPrice元，封顶topPrice元
        /// </summary>
        [XmlElement("per_price")]
        public string PerPrice { get; set; }

        /// <summary>
        /// 当券类型是代金券的时候，这个字段代表实际金额；当券类型是减至券的时候，这个字段代表减至到的金额
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 券推荐语
        /// </summary>
        [XmlElement("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// 买A送B中，B的描述
        /// </summary>
        [XmlElement("send_item_name")]
        public string SendItemName { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 券的店铺名
        /// </summary>
        [XmlElement("shop_name")]
        public string ShopName { get; set; }

        /// <summary>
        /// 已领数
        /// </summary>
        [XmlElement("sold")]
        public string Sold { get; set; }

        /// <summary>
        /// 优惠开始时间
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 每满thresholdPrice元减perPrice元，封顶topPrice元
        /// </summary>
        [XmlElement("threshold_price")]
        public string ThresholdPrice { get; set; }

        /// <summary>
        /// 每满减用的字段：每满thresholdPrice元减perPrice元，封顶topPrice元
        /// </summary>
        [XmlElement("top_price")]
        public string TopPrice { get; set; }

        /// <summary>
        /// 目前有discount:折扣券;cash:代金券;exchange:兑换券;limit_reduce_cash:减至券
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// 券二级类型。all_discount:全场折扣;single_discount:单品折扣;all_cash:全场代金;single_cash：单品代金
        /// </summary>
        [XmlElement("vol_type")]
        public string VolType { get; set; }
    }
}
