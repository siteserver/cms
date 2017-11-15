using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipaySecurityProdAmlriskQueryResponse.
    /// </summary>
    public class AlipaySecurityProdAmlriskQueryResponse : AopResponse
    {
        /// <summary>
        /// 事件ID，由入参得来
        /// </summary>
        [XmlElement("event_id")]
        public string EventId { get; set; }

        /// <summary>
        /// 根据反洗钱风险检查，该商户是否有风险，有风险则为Yes，无风险则为No
        /// </summary>
        [XmlElement("has_risk")]
        public string HasRisk { get; set; }

        /// <summary>
        /// 商户ID，由入参得来
        /// </summary>
        [XmlElement("merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// 当发现有风险时，以列表形式表示风险详情，风险点可以有0个至多个。
        /// </summary>
        [XmlArray("risks")]
        [XmlArrayItem("merchant_screen_hit")]
        public List<MerchantScreenHit> Risks { get; set; }
    }
}
