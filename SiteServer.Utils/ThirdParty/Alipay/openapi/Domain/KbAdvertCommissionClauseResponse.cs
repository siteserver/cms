using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertCommissionClauseResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertCommissionClauseResponse : AopObject
    {
        /// <summary>
        /// 比例分佣规则  只有type=PERCENTAGE_CLAUSE才会有值
        /// </summary>
        [XmlElement("percentage_clause")]
        public KbAdvertCommissionClausePercentageResponse PercentageClause { get; set; }

        /// <summary>
        /// 定额分佣规则  只有type=QUOTA_CLAUSE才会有值
        /// </summary>
        [XmlElement("quota_clause")]
        public KbAdvertCommissionClauseQuotaResponse QuotaClause { get; set; }

        /// <summary>
        /// 分佣规则类型  PERCENTAGE_CLAUSE-比例  QUOTA_CLAUSE-定额
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
