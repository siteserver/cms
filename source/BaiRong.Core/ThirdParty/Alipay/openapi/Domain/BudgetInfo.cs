using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BudgetInfo Data Structure.
    /// </summary>
    [Serializable]
    public class BudgetInfo : AopObject
    {
        /// <summary>
        /// 预算数量
        /// </summary>
        [XmlElement("budget_total")]
        public string BudgetTotal { get; set; }

        /// <summary>
        /// 预算类型
        /// </summary>
        [XmlElement("budget_type")]
        public string BudgetType { get; set; }
    }
}
