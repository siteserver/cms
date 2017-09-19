using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsMarketingDiscountDecisionModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsMarketingDiscountDecisionModel : AopObject
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
        /// 优惠咨询因子
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
        /// 营销标的列表
        /// </summary>
        [XmlArray("mkt_objects")]
        [XmlArrayItem("ins_mkt_object_d_t_o")]
        public List<InsMktObjectDTO> MktObjects { get; set; }

        /// <summary>
        /// 请求流水Id
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}
