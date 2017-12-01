using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertCommissionClauseQuota Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertCommissionClauseQuota : AopObject
    {
        /// <summary>
        /// 定额结束范围(精度2位的非负小数)
        /// </summary>
        [XmlElement("quota_amount_end")]
        public string QuotaAmountEnd { get; set; }

        /// <summary>
        /// 定额开始范围(精度2位的非负小数)
        /// </summary>
        [XmlElement("quota_amount_start")]
        public string QuotaAmountStart { get; set; }
    }
}
