using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketProductQuerydetailResponse.
    /// </summary>
    public class AlipayOfflineMarketProductQuerydetailResponse : AopResponse
    {
        /// <summary>
        /// 商品首图，尺寸比例在65:53范围内且图片大小不超过10k皆可
        /// </summary>
        [XmlElement("cover")]
        public string Cover { get; set; }

        /// <summary>
        /// 商品描述，只有单品代金券有商品描述
        /// </summary>
        [XmlArray("descriptions")]
        [XmlArrayItem("alipay_data_item_description")]
        public List<AlipayDataItemDescription> Descriptions { get; set; }

        /// <summary>
        /// 商品下架时间，不得早于商品生效时间，商品下架
        /// </summary>
        [XmlElement("gmt_end")]
        public string GmtEnd { get; set; }

        /// <summary>
        /// 商品生效时间，到达生效时间后才可在客户端展示出来
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 商品库存数量
        /// </summary>
        [XmlElement("inventory")]
        public long Inventory { get; set; }

        /// <summary>
        /// 是否自动延期，默认false
        /// </summary>
        [XmlElement("is_auto_expanded")]
        public bool IsAutoExpanded { get; set; }

        /// <summary>
        /// 该商品当前的状态，共有四个状态：  INIT（初始化状态）  EFFECTIVE（上架/售卖中）  PAUSE（暂停售卖）  INVALID（下架/失效状态）  说明： 处于暂停售卖状态的商品还可以变成上架/售卖中状态，但一旦INVALID就相当于删除了该商品，无法再做其他操作
        /// </summary>
        [XmlElement("item_status")]
        public string ItemStatus { get; set; }

        /// <summary>
        /// 商品类型，券类型为固定值VOUCHER
        /// </summary>
        [XmlElement("item_type")]
        public string ItemType { get; set; }

        /// <summary>
        /// 图片集，是map转化成的json串，key是图片id,value是图片url
        /// </summary>
        [XmlElement("pic_coll")]
        public string PicColl { get; set; }

        /// <summary>
        /// 商品购买类型：  OBTAIN为领取  AUTO_OBTAIN为自动领取
        /// </summary>
        [XmlElement("purchase_mode")]
        public string PurchaseMode { get; set; }

        /// <summary>
        /// 销售规则，如用户一天只能领取一次等
        /// </summary>
        [XmlElement("sales_rule")]
        public AlipayDataItemSalesRule SalesRule { get; set; }

        /// <summary>
        /// 上架门店id列表，即传入一个或多个shop_id，必须是创建商品partnerId下的店铺，目前支持的店铺最大100个，如果超过100个店铺需要报备
        /// </summary>
        [XmlArray("shop_list")]
        [XmlArrayItem("string")]
        public List<string> ShopList { get; set; }

        /// <summary>
        /// 商品名称，请勿超过15个汉字，30个字符
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 券模板信息
        /// </summary>
        [XmlElement("voucher_templete")]
        public AlipayDataItemVoucherTemplete VoucherTemplete { get; set; }

        /// <summary>
        /// 商品顺序权重，必须是整数，不传默认为0，权重数值越大排序越靠前
        /// </summary>
        [XmlElement("weight")]
        public long Weight { get; set; }
    }
}
