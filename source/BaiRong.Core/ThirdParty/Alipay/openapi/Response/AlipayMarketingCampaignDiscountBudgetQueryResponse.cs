using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCampaignDiscountBudgetQueryResponse.
    /// </summary>
    public class AlipayMarketingCampaignDiscountBudgetQueryResponse : AopResponse
    {
        /// <summary>
        /// 预算ID
        /// </summary>
        [XmlElement("budget_id")]
        public string BudgetId { get; set; }

        /// <summary>
        /// 预算总金额，单位：元
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }

        /// <summary>
        /// 已使用金额
        /// </summary>
        [XmlElement("used_amount")]
        public string UsedAmount { get; set; }
    }
}
