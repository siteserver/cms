using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCampaignDiscountBudgetCreateResponse.
    /// </summary>
    public class AlipayMarketingCampaignDiscountBudgetCreateResponse : AopResponse
    {
        /// <summary>
        /// 预算ID
        /// </summary>
        [XmlElement("budget_id")]
        public string BudgetId { get; set; }
    }
}
