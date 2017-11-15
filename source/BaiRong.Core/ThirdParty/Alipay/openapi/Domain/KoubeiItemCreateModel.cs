using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiItemCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiItemCreateModel : AopObject
    {
        /// <summary>
        /// 服务商、服务商员工、商户、商户员工等口碑角色操作时必填，对应为《koubei.member.data.oauth.query》中的auth_code，默认有效期24小时；isv自身角色操作的时候，无需传该参数
        /// </summary>
        [XmlElement("auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 口碑商品所属的后台类目id，ISV可通过开放接口koubei.item.category.children.batchquery来获得后台类目树，并选择叶子类目，作为该值传入
        /// </summary>
        [XmlElement("category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 商品首图。支持bmp,png,jpeg,jpg,gif格式的图片，建议宽高比16:9，建议宽高：1242*698px 图片大小≤5M。图片大小超过5M,接口会报错。若图片尺寸不对，口碑服务器自身不会做压缩，但是口碑把这些图片放到客户端上展现时，自己会做性能优化(等比缩放，以图片中心为基准裁剪)。
        /// </summary>
        [XmlElement("cover")]
        public string Cover { get; set; }

        /// <summary>
        /// 商品描述，列表类型，最多10项，每一项的key，value的描述见下面两行
        /// </summary>
        [XmlArray("descriptions")]
        [XmlArrayItem("koubei_item_description")]
        public List<KoubeiItemDescription> Descriptions { get; set; }

        /// <summary>
        /// 商品生效时间，商品状态有效并且到达生效时间后才可在客户端（消费者端）展示出来，如果商品生效时间小于当前时间，则立即生效。  说明：商品的生效时间不能早于创建当天的0点
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 商品库存数量，标准商品必填；非标准商品不需要填写，不填写则默认为：99999999
        /// </summary>
        [XmlElement("inventory")]
        public long Inventory { get; set; }

        /// <summary>
        /// 非标准商品详情页url，用户通过此url跳转到非标准商品的商品详情页，非标准商品必填
        /// </summary>
        [XmlElement("item_detail_url")]
        public string ItemDetailUrl { get; set; }

        /// <summary>
        /// 商品类型为交易凭证类型：TRADE_VOUCHER
        /// </summary>
        [XmlElement("item_type")]
        public string ItemType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 操作上下文 isv角色操作时必填。其他角色不需填写，不填时以auth_code为准。
        /// </summary>
        [XmlElement("operation_context")]
        public KoubeiOperationContext OperationContext { get; set; }

        /// <summary>
        /// 标准商品为原价，必填。非标准商品请勿填写，填写无效。价格单位为元
        /// </summary>
        [XmlElement("original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// 商品详情图。尺寸大小与cover一致，最多5张，以英文逗号分隔  端上展现时，自己会做性能优化(等比缩放，以图片中心为基准裁剪)
        /// </summary>
        [XmlElement("picture_details")]
        public string PictureDetails { get; set; }

        /// <summary>
        /// 标准商品为现价,必填。非标准商品为最小价格（非标商品为xx元起）必填。价格单位为元。如果现价与原价相等时，则会以原价售卖，并且客户端只展示一个价格（原价）
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 标准商品:FIX；非标准商品:FLOAT
        /// </summary>
        [XmlElement("price_mode")]
        public string PriceMode { get; set; }

        /// <summary>
        /// 支持英文字母和数字，由开发者自行定义（不允许重复），在商品notify消息中也会带有该参数，以此标明本次notify消息是对哪个请求的回应
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 上架门店id列表，即传入一个或多个shop_id。多个ID则以英文分隔
        /// </summary>
        [XmlElement("shop_ids")]
        public string ShopIds { get; set; }

        /// <summary>
        /// 商品名称，请勿超过40汉字，80个字符
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 交易凭证类商品模板信息
        /// </summary>
        [XmlElement("trade_voucher_item_template")]
        public KoubeiTradeVoucherItemTemplete TradeVoucherItemTemplate { get; set; }

        /// <summary>
        /// 商品顺序权重，必须是整数，不传默认为0，权重数值越大排序越靠前
        /// </summary>
        [XmlElement("weight")]
        public string Weight { get; set; }
    }
}
