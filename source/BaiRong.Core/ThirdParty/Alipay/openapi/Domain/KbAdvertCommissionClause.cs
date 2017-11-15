using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertCommissionClause Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertCommissionClause : AopObject
    {
        /// <summary>
        /// 条款类型（条款类型是什么，下面填的条款就是什么）  PERCENTAGE_CLAUSE：比例分佣条款  QUOTA_CLAUSE：固定金额  MISSION_CLAIM_CLAUSE:专属认领人条款
        /// </summary>
        [XmlElement("clause_type")]
        public string ClauseType { get; set; }

        /// <summary>
        /// 比例分佣条款
        /// </summary>
        [XmlElement("percentage_clause")]
        public KbAdvertPercentageCommissionClause PercentageClause { get; set; }

        /// <summary>
        /// 专属人员条款
        /// </summary>
        [XmlElement("preserve_clause")]
        public KbAdvertPreserveCommissionClause PreserveClause { get; set; }

        /// <summary>
        /// 固定金额条款
        /// </summary>
        [XmlElement("quota_clause")]
        public KbAdvertQuotaCommissionClause QuotaClause { get; set; }
    }
}
