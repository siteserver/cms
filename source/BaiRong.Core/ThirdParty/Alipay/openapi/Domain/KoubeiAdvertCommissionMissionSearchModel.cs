using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionMissionSearchModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionMissionSearchModel : AopObject
    {
        /// <summary>
        /// 分佣规则类型(枚举值key对应于请求对象中复杂类型的key)  percentage_clause-比例  quota_clause-定额
        /// </summary>
        [XmlElement("commission_clause_type")]
        public string CommissionClauseType { get; set; }

        /// <summary>
        /// 当前页码，默认1
        /// </summary>
        [XmlElement("page_index")]
        public string PageIndex { get; set; }

        /// <summary>
        /// 每页记录数，默认10，最大100
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }

        /// <summary>
        /// 比例分佣规则  只有commission_clause_type=percentage_clause才能传值
        /// </summary>
        [XmlElement("percentage_clause")]
        public KbAdvertCommissionClausePercentage PercentageClause { get; set; }

        /// <summary>
        /// 定额分佣规则  只有commission_clause_type=quota_clause才能传值
        /// </summary>
        [XmlElement("quota_clause")]
        public KbAdvertCommissionClauseQuota QuotaClause { get; set; }

        /// <summary>
        /// 任务类型(枚举值key对应于请求对象中复杂类型的key)  voucher-券
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// 券任务(支持模糊匹配)  只有type=voucher才能传值
        /// </summary>
        [XmlElement("voucher")]
        public KbAdvertSubjectVoucher Voucher { get; set; }
    }
}
