using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CaterItemListInfo Data Structure.
    /// </summary>
    [Serializable]
    public class CaterItemListInfo : AopObject
    {
        /// <summary>
        /// 商品最后修改时间。格式为YYYY-MM-DD HH:mm:ss
        /// </summary>
        [XmlElement("gmt_modified")]
        public string GmtModified { get; set; }

        /// <summary>
        /// 商品剩余库存数量
        /// </summary>
        [XmlElement("inventory")]
        public long Inventory { get; set; }

        /// <summary>
        /// 口碑体系内部商品的唯一标识
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 当前商品状态。状态枚举值为：INIT表示初始状态， EFFECTIVE表示生效状态，PAUSE表示暂停售卖，FREEZE表示冻结，INVALID表示失效状态
        /// </summary>
        [XmlElement("item_status")]
        public string ItemStatus { get; set; }

        /// <summary>
        /// 商品原价。字符串，单位元，2位小数
        /// </summary>
        [XmlElement("original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// 商品现价。字符串，单位元，两位小数
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 已退回商品退回原因
        /// </summary>
        [XmlElement("reject_reason")]
        public string RejectReason { get; set; }

        /// <summary>
        /// 商品名称，请勿超过40汉字，80个字符
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 购买有效期：商品自购买起多长时间内有效，取值范围 7-360，单位天。举例，如果是7的话，是到第七天晚上23:59:59失效。商品购买后，没有在有效期内核销，则自动退款给用户。举例：买了一个鱼香肉丝杨梅汁套餐的商品，有效期一个月，如果一个月之后，用户没有消费该套餐，则自动退款给用户
        /// </summary>
        [XmlElement("validity_period")]
        public long ValidityPeriod { get; set; }
    }
}
