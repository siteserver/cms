using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ItemQueryResponse Data Structure.
    /// </summary>
    [Serializable]
    public class ItemQueryResponse : AopObject
    {
        /// <summary>
        /// 口碑商品所属的后台类目id，后台类目数据来源：开放接口koubei.item.category.children.batchquery（查询后台类目树接口）
        /// </summary>
        [XmlElement("category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 首图
        /// </summary>
        [XmlElement("cover")]
        public string Cover { get; set; }

        /// <summary>
        /// 商品描述，列表类型，每一项的key，value的描述见下面两行
        /// </summary>
        [XmlArray("descriptions")]
        [XmlArrayItem("koubei_item_description")]
        public List<KoubeiItemDescription> Descriptions { get; set; }

        /// <summary>
        /// 商品生效时间，商品状态有效并且到达生效时间后才可在客户端（消费者端）展示出来，如果商品生效时间小于当前时间，则立即生效。  说明： 商品的生效时间不能早于创建当天的0点
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 当前库存
        /// </summary>
        [XmlElement("inventory")]
        public long Inventory { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 该商品当前的状态，共有5个状态：INIT（初始状态）EFFECTIVE（生效）PAUSE（暂停）FREEZE(冻结)INVALID（失效）；详见状态变更图
        /// </summary>
        [XmlElement("item_status")]
        public string ItemStatus { get; set; }

        /// <summary>
        /// 商品类型，交易凭证类：TRADE_VOUCHER
        /// </summary>
        [XmlElement("item_type")]
        public string ItemType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 标准商品为原价，必填。非标准商品请勿填写，填写无效。价格单位为元
        /// </summary>
        [XmlElement("original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// 图片集，本商品所有图片id和URL的对应关系数组
        /// </summary>
        [XmlArray("pic_coll")]
        [XmlArrayItem("string")]
        public List<string> PicColl { get; set; }

        /// <summary>
        /// 商品详情图，多张图片以英文逗号分隔
        /// </summary>
        [XmlElement("picture_details")]
        public string PictureDetails { get; set; }

        /// <summary>
        /// 标准商品为现价,选填。非标准商品为最小价格（非标商品为xx元起）必填。价格单位为元。如果标准商品现价不填写，则以原价进行售卖；如果现价与原价相等时，则会以原价售卖，并且客户端只展示一个价格（原价）
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 标准商品:FIX；非标准商品:FLOAT ，根据该字段判断商品是标准商品或非标商品。
        /// </summary>
        [XmlElement("price_mode")]
        public string PriceMode { get; set; }

        /// <summary>
        /// 适用门店列表
        /// </summary>
        [XmlElement("shop_ids")]
        public string ShopIds { get; set; }

        /// <summary>
        /// 商品名称，不超过20汉字，40个字符
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 交易凭证类商品模板信息
        /// </summary>
        [XmlElement("trade_voucher_item_template")]
        public KoubeiTradeVoucherItemTemplete TradeVoucherItemTemplate { get; set; }

        /// <summary>
        /// 商品顺序权重
        /// </summary>
        [XmlElement("weight")]
        public long Weight { get; set; }
    }
}
