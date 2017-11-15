using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiCateringItemCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiCateringItemCreateModel : AopObject
    {
        /// <summary>
        /// 服务商、服务商员工、商户、商户员工等口碑角色操作时必填，对应为《koubei.member.data.oauth.query》中的auth_code，默认有效期24小时；isv自身角色操作的时候，无需传该参数
        /// </summary>
        [XmlElement("auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 商品可用时段列表。最多添加三条规则。该内容仅用于展示，不影响实际核销。
        /// </summary>
        [XmlArray("available_periods")]
        [XmlArrayItem("available_period_info")]
        public List<AvailablePeriodInfo> AvailablePeriods { get; set; }

        /// <summary>
        /// 商品购买须知
        /// </summary>
        [XmlElement("buyer_notes")]
        public BuyerNotesInfo BuyerNotes { get; set; }

        /// <summary>
        /// 口碑商品所属的后台类目id，ISV可通过开放接口koubei.item.category.children.batchquery来获得后台类目树，并选择叶子类目，作为该值传入
        /// </summary>
        [XmlElement("category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 商品首图。支持bmp,png,jpeg,jpg,gif格式的图片，建议宽高比16:9，建议宽高：1242*698px 图片大小≤5M。图片大小超过5M,接口会报错。若图片尺寸不对，口碑服务器自身不会做压缩，但是口碑把这些图片放到客户端上展现时，自己会做性能优化(等比缩放，以图片中心为基准裁剪)
        /// </summary>
        [XmlElement("cover")]
        public string Cover { get; set; }

        /// <summary>
        /// 商品生效时间，商品状态有效并且到达生效时间后才可在客户端（消费者端）展示出来，如果商品生效时间小于当前时间，则立即生效。  说明：商品的生效时间不能早于创建当天的0点
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 发布商品库存数量
        /// </summary>
        [XmlElement("inventory")]
        public long Inventory { get; set; }

        /// <summary>
        /// 商品详情-菜品图文详情
        /// </summary>
        [XmlArray("item_dishes")]
        [XmlArrayItem("item_dish_info")]
        public List<ItemDishInfo> ItemDishes { get; set; }

        /// <summary>
        /// 商品详情-商品套餐内容
        /// </summary>
        [XmlArray("item_packages")]
        [XmlArrayItem("item_package_info")]
        public List<ItemPackageInfo> ItemPackages { get; set; }

        /// <summary>
        /// 商家公告，最多不超过100个汉字，200个字符
        /// </summary>
        [XmlArray("latest_notice")]
        [XmlArrayItem("string")]
        public List<string> LatestNotice { get; set; }

        /// <summary>
        /// 商品备注信息。用于商户内部管理，用户页面不露出。
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商品详情-商家介绍图文详情
        /// </summary>
        [XmlElement("merchant_introduction")]
        public IntroductionInfo MerchantIntroduction { get; set; }

        /// <summary>
        /// 操作人员身份类型。如果是isv代操作，请传入ISV；如果是商户操作请传入MERCHANT；如果是商户员工则传入M_STAFF
        /// </summary>
        [XmlElement("operator_type")]
        public string OperatorType { get; set; }

        /// <summary>
        /// 商品原价。字符串类型，单位元，2位小数。最高价格49998元
        /// </summary>
        [XmlElement("original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// 商品详情-套餐补充说明列表
        /// </summary>
        [XmlArray("package_notes")]
        [XmlArrayItem("string")]
        public List<string> PackageNotes { get; set; }

        /// <summary>
        /// 商品详情图片列表。尺寸大小与商品首图一致，最多5张。C端上展现时，自己会做性能优化(等比缩放，以图片中心为基准裁剪)
        /// </summary>
        [XmlArray("picture_details")]
        [XmlArrayItem("string")]
        public List<string> PictureDetails { get; set; }

        /// <summary>
        /// 商品现价（优惠价）。字符串类型，单位元，2位小数。最高价格49998元
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 支持英文字母和数字，由开发者自行定义（不允许重复），在商品notify消息中也会带有该参数，以此标明本次notify消息是对哪个请求的回应
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 商品需要关联的门店id列表，即传入一个或多个shop_id。
        /// </summary>
        [XmlArray("shop_ids")]
        [XmlArrayItem("string")]
        public List<string> ShopIds { get; set; }

        /// <summary>
        /// 商品编码，由商家自定义，不可重复，用于商品核销
        /// </summary>
        [XmlElement("sku_id")]
        public string SkuId { get; set; }

        /// <summary>
        /// 商品名称，请勿超过40汉字，80个字符
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 商品不可用日期区间。该内容仅用于展示，不影响实际核销。
        /// </summary>
        [XmlArray("unavailable_periods")]
        [XmlArrayItem("unavailable_period_info")]
        public List<UnavailablePeriodInfo> UnavailablePeriods { get; set; }

        /// <summary>
        /// 购买有效期：商品自购买起多长时间内有效，取值范围 7-360，单位天。举例，如果是7的话，是到第七天晚上23:59:59失效。商品购买后，没有在有效期内核销，则自动退款给用户。举例：买了一个鱼香肉丝杨梅汁套餐的商品，有效期一个月，如果一个月之后，用户没有消费该套餐，则自动退款给用户
        /// </summary>
        [XmlElement("validity_period")]
        public long ValidityPeriod { get; set; }

        /// <summary>
        /// 商品顺序权重，必须是整数，不传默认为0，权重数值越大排序越靠前
        /// </summary>
        [XmlElement("weight")]
        public string Weight { get; set; }
    }
}
