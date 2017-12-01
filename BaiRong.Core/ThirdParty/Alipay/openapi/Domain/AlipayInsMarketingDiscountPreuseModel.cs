using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsMarketingDiscountPreuseModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsMarketingDiscountPreuseModel : AopObject
    {
        /// <summary>
        /// 保险营销账号Id
        /// </summary>
        [XmlElement("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// 保险营销账号类型
        /// </summary>
        [XmlElement("account_type")]
        public long AccountType { get; set; }

        /// <summary>
        /// 保险营销业务类型
        /// </summary>
        [XmlElement("business_type")]
        public long BusinessType { get; set; }

        /// <summary>
        /// 优惠决策因子
        /// </summary>
        [XmlArray("factors")]
        [XmlArrayItem("ins_mkt_factor_d_t_o")]
        public List<InsMktFactorDTO> Factors { get; set; }

        /// <summary>
        /// 营销市场列表
        /// </summary>
        [XmlArray("market_types")]
        [XmlArrayItem("number")]
        public List<long> MarketTypes { get; set; }

        /// <summary>
        /// 权益活动列表
        /// </summary>
        [XmlArray("mkt_coupon_campaigns")]
        [XmlArrayItem("ins_mkt_coupon_cmpgn_base_d_t_o")]
        public List<InsMktCouponCmpgnBaseDTO> MktCouponCampaigns { get; set; }

        /// <summary>
        /// 用户选择的权益列表
        /// </summary>
        [XmlArray("mkt_coupons")]
        [XmlArrayItem("ins_mkt_coupon_base_d_t_o")]
        public List<InsMktCouponBaseDTO> MktCoupons { get; set; }

        /// <summary>
        /// 营销标的列表
        /// </summary>
        [XmlArray("mkt_objects")]
        [XmlArrayItem("ins_mkt_object_d_t_o")]
        public List<InsMktObjectDTO> MktObjects { get; set; }

        /// <summary>
        /// 请求流水id
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}
