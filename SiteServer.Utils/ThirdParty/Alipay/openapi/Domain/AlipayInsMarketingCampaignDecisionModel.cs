using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsMarketingCampaignDecisionModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsMarketingCampaignDecisionModel : AopObject
    {
        /// <summary>
        /// 描述营销活动涉及的业务类型  1：平台险业务
        /// </summary>
        [XmlElement("business_type")]
        public long BusinessType { get; set; }

        /// <summary>
        /// 营销市场列表
        /// </summary>
        [XmlArray("market_types")]
        [XmlArrayItem("number")]
        public List<long> MarketTypes { get; set; }

        /// <summary>
        /// 保险营销平台营销标的标识
        /// </summary>
        [XmlElement("mkt_obj_id")]
        public string MktObjId { get; set; }

        /// <summary>
        /// 保险营销平台的营销标的类型  1：淘宝商品
        /// </summary>
        [XmlElement("mkt_obj_type")]
        public long MktObjType { get; set; }

        /// <summary>
        /// 请求流水id
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}
