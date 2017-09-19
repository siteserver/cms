using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ShopRec Data Structure.
    /// </summary>
    [Serializable]
    public class ShopRec : AopObject
    {
        /// <summary>
        /// 店铺地址
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        [XmlElement("brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 商圈
        /// </summary>
        [XmlElement("busi_area")]
        public string BusiArea { get; set; }

        /// <summary>
        /// 城市id
        /// </summary>
        [XmlElement("city_id")]
        public string CityId { get; set; }

        /// <summary>
        /// 菜系
        /// </summary>
        [XmlElement("cuisine")]
        public string Cuisine { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        [XmlElement("distance")]
        public string Distance { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 是否有优惠
        /// </summary>
        [XmlElement("has_hui")]
        public string HasHui { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [XmlElement("head_shop_name")]
        public string HeadShopName { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 人气分
        /// </summary>
        [XmlElement("popularity")]
        public string Popularity { get; set; }

        /// <summary>
        /// 人气等级
        /// </summary>
        [XmlElement("popularity_level")]
        public string PopularityLevel { get; set; }

        /// <summary>
        /// 人均消费
        /// </summary>
        [XmlElement("price_average")]
        public string PriceAverage { get; set; }

        /// <summary>
        /// 前台一级类目列表
        /// </summary>
        [XmlElement("root_display_category_info")]
        public string RootDisplayCategoryInfo { get; set; }

        /// <summary>
        /// 店铺跳转链接
        /// </summary>
        [XmlElement("shop_detail_url")]
        public string ShopDetailUrl { get; set; }

        /// <summary>
        /// 店铺id
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 店铺logo图
        /// </summary>
        [XmlElement("shop_logo_url")]
        public string ShopLogoUrl { get; set; }

        /// <summary>
        /// 店铺详细名称
        /// </summary>
        [XmlElement("shop_name")]
        public string ShopName { get; set; }

        /// <summary>
        /// 推荐语
        /// </summary>
        [XmlElement("shop_recommend_one_tag_compose")]
        public string ShopRecommendOneTagCompose { get; set; }

        /// <summary>
        /// 推荐店铺的券
        /// </summary>
        [XmlArray("voucher_list")]
        [XmlArrayItem("voucher_rec")]
        public List<VoucherRec> VoucherList { get; set; }
    }
}
