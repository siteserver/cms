using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// VoucherUserExternalTradeInfo Data Structure.
    /// </summary>
    [Serializable]
    public class VoucherUserExternalTradeInfo : AopObject
    {
        /// <summary>
        /// 核销金额
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 核销时间
        /// </summary>
        [XmlElement("consume_date")]
        public string ConsumeDate { get; set; }

        /// <summary>
        /// 券核销的门店id
        /// </summary>
        [XmlElement("consume_shop_id")]
        public string ConsumeShopId { get; set; }

        /// <summary>
        /// 外部交易号
        /// </summary>
        [XmlElement("external_trade_no")]
        public string ExternalTradeNo { get; set; }
    }
}
