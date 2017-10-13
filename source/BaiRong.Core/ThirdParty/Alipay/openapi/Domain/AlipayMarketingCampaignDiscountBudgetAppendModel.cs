using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignDiscountBudgetAppendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignDiscountBudgetAppendModel : AopObject
    {
        /// <summary>
        /// 预算ID
        /// </summary>
        [XmlElement("budget_id")]
        public string BudgetId { get; set; }

        /// <summary>
        /// 追加后的预算总金额（注意：是追加后的预算总金额，不是在原基础上追加的金额），单位：元
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }
    }
}
