using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PricingVO Data Structure.
    /// </summary>
    [Serializable]
    public class PricingVO : AopObject
    {
        /// <summary>
        /// 买入价
        /// </summary>
        [XmlElement("bid")]
        public string Bid { get; set; }

        /// <summary>
        /// 基准币种
        /// </summary>
        [XmlElement("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 基本币种单位
        /// </summary>
        [XmlElement("currency_unit")]
        public long CurrencyUnit { get; set; }

        /// <summary>
        /// 汇率失效时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("expiry_timestamp")]
        public string ExpiryTimestamp { get; set; }

        /// <summary>
        /// 汇率生成时间 用来做幂等 yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("generate_timestamp")]
        public string GenerateTimestamp { get; set; }

        /// <summary>
        /// 远期或者掉期到期时间 yyyyMMdd
        /// </summary>
        [XmlElement("maturity_date")]
        public string MaturityDate { get; set; }

        /// <summary>
        /// 该价格的最大买入量
        /// </summary>
        [XmlElement("maximum_bid_amount")]
        public long MaximumBidAmount { get; set; }

        /// <summary>
        /// 该价格的最大卖出量
        /// </summary>
        [XmlElement("maximum_offer_amount")]
        public long MaximumOfferAmount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 中间价
        /// </summary>
        [XmlElement("mid")]
        public string Mid { get; set; }

        /// <summary>
        /// 该价格的最小买入量
        /// </summary>
        [XmlElement("minimum_bid_amount")]
        public long MinimumBidAmount { get; set; }

        /// <summary>
        /// 该价格的最小卖出量
        /// </summary>
        [XmlElement("minimum_offer_amount")]
        public long MinimumOfferAmount { get; set; }

        /// <summary>
        /// 卖出价
        /// </summary>
        [XmlElement("offer")]
        public string Offer { get; set; }

        /// <summary>
        /// 标准期限TODAY TOM SPOT 1D 1W 1M 1Y
        /// </summary>
        [XmlElement("period")]
        public string Period { get; set; }

        /// <summary>
        /// 源汇率参考id 唯一id
        /// </summary>
        [XmlElement("rate_reference_id")]
        public string RateReferenceId { get; set; }

        /// <summary>
        /// 汇率类型 SPOT FORWARD
        /// </summary>
        [XmlElement("rate_type")]
        public string RateType { get; set; }

        /// <summary>
        /// 即期买入价
        /// </summary>
        [XmlElement("spot_bid")]
        public string SpotBid { get; set; }

        /// <summary>
        /// 即期中间价
        /// </summary>
        [XmlElement("spot_mid")]
        public string SpotMid { get; set; }

        /// <summary>
        /// 即期卖出价
        /// </summary>
        [XmlElement("spot_offer")]
        public string SpotOffer { get; set; }

        /// <summary>
        /// 汇率生效时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("start_timestamp")]
        public string StartTimestamp { get; set; }

        /// <summary>
        /// 货币对
        /// </summary>
        [XmlElement("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// 汇率缓冲时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("threshold_timestamp")]
        public string ThresholdTimestamp { get; set; }

        /// <summary>
        /// 锁汇失效时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("valid_timestamp")]
        public string ValidTimestamp { get; set; }
    }
}
