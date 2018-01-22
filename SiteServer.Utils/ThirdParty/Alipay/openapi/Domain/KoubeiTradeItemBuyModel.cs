using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiTradeItemBuyModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiTradeItemBuyModel : AopObject
    {
        /// <summary>
        /// 预定用户的联系号码。要求合法的手机号码或者座机；该字段仅用于商品预定，商品预定场景为必填字段。如：0579-XXXXXXX；1526XXXXXXX
        /// </summary>
        [XmlElement("buyer_phone_number")]
        public string BuyerPhoneNumber { get; set; }

        /// <summary>
        /// 预定的买家用户名称；该字段仅用于商品预定，商品预定场景下为必填字段
        /// </summary>
        [XmlElement("buyer_user_name")]
        public string BuyerUserName { get; set; }

        /// <summary>
        /// 若无现价则此值传商品原价，交易创建将根据此价格进行售卖。  传入的价格最多可有两位小数，最大值不可超过5000，超过则会报错。
        /// </summary>
        [XmlElement("current_price")]
        public string CurrentPrice { get; set; }

        /// <summary>
        /// 额外描述信息，比如预定时间信息，需要以“字段1：描述1；字段2：描述2；....“方式传入。标点符号限制集如下,不能传下列标点之外的标点符号：..！.{},:()"[]，。!！,/>"{},:"，?？。!！\[\]]=+_@#$%*
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 原价，传入的价格最多可有两位小数，超过则会报错
        /// </summary>
        [XmlElement("original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// 外部业务流水编号,推荐:yyyymmddhhmmssSSS99999999(年月日时分秒+8位随机码)，开发者可根据该编号与口碑订单一一对应。本订单创建行为的流水ID,用于平台做幂等控制
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 商户pid
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 购买数量，最大传入20，否则下单页会报错
        /// </summary>
        [XmlElement("quantity")]
        public long Quantity { get; set; }

        /// <summary>
        /// 预定结束时间；该字段仅用于商品预定，商品预定场景下为非必填字段。  格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("reserve_end_time")]
        public string ReserveEndTime { get; set; }

        /// <summary>
        /// 预定开始时间；该字段仅用于商品预定，商品预定场景下为必填字段  格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("reserve_start_time")]
        public string ReserveStartTime { get; set; }

        /// <summary>
        /// 店铺ID，用于后续统计商家各门店的售卖，需传入口碑店铺id，取值规则见FAQ常见问题。https://doc.open.alipay.com/docs/doc.htm?&docType=1&articleId=105746
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}
