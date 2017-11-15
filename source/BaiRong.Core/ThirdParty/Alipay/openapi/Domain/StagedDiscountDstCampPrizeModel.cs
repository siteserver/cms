using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// StagedDiscountDstCampPrizeModel Data Structure.
    /// </summary>
    [Serializable]
    public class StagedDiscountDstCampPrizeModel : AopObject
    {
        /// <summary>
        /// 折扣预算ID
        /// </summary>
        [XmlElement("budget_id")]
        public string BudgetId { get; set; }

        /// <summary>
        /// 折扣幅度列表.
        /// </summary>
        [XmlArray("discount_rate_model_list")]
        [XmlArrayItem("discount_rate_model")]
        public List<DiscountRateModel> DiscountRateModelList { get; set; }

        /// <summary>
        /// 奖品id
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 单次优惠上限(元)
        /// </summary>
        [XmlElement("max_discount_amt")]
        public string MaxDiscountAmt { get; set; }
    }
}
