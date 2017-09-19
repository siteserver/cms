using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbadvertCommissionLimit Data Structure.
    /// </summary>
    [Serializable]
    public class KbadvertCommissionLimit : AopObject
    {
        /// <summary>
        /// 推广者类型
        /// </summary>
        [XmlElement("commission_user_type")]
        public string CommissionUserType { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        [XmlElement("level")]
        public long Level { get; set; }

        /// <summary>
        /// 比例分佣的最大金额
        /// </summary>
        [XmlElement("max_max_amount")]
        public string MaxMaxAmount { get; set; }

        /// <summary>
        /// 固定金额上限
        /// </summary>
        [XmlElement("max_quota_amount")]
        public string MaxQuotaAmount { get; set; }

        /// <summary>
        /// 最小分佣比例
        /// </summary>
        [XmlElement("min_commission_percentage")]
        public string MinCommissionPercentage { get; set; }

        /// <summary>
        /// 最小固定金额
        /// </summary>
        [XmlElement("min_quota_amount")]
        public string MinQuotaAmount { get; set; }
    }
}
