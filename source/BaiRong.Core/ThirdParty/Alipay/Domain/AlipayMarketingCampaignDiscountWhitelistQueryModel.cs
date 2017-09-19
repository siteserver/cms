using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignDiscountWhitelistQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignDiscountWhitelistQueryModel : AopObject
    {
        /// <summary>
        /// 活动od
        /// </summary>
        [XmlElement("camp_id")]
        public string CampId { get; set; }
    }
}
