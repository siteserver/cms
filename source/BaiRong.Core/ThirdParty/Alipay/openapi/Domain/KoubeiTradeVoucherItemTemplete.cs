using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiTradeVoucherItemTemplete Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiTradeVoucherItemTemplete : AopObject
    {
        /// <summary>
        /// 购买须知，列表类型，最多10项
        /// </summary>
        [XmlArray("buyer_notes")]
        [XmlArrayItem("koubei_item_description")]
        public List<KoubeiItemDescription> BuyerNotes { get; set; }

        /// <summary>
        /// 表示是否支持预定，支持“T”, 不支持“F”
        /// </summary>
        [XmlElement("support_book")]
        public string SupportBook { get; set; }

        /// <summary>
        /// 购买有效期：商品自购买起多长时间内有效，取值范围 7-360，单位天。举例，如果是7的话，是到第七天晚上23:59:59失效。商品购买后，没有在有效期内核销，则自动退款给用户。举例：买了一个高级造型师洗剪吹的商品，有效期一个月，如果一个月之后，用户没有使用商品来进行洗剪吹的服务，则自动退款给用户。
        /// </summary>
        [XmlElement("validity_period")]
        public string ValidityPeriod { get; set; }
    }
}
