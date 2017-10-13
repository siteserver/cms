using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingDataCustomreportSaveModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingDataCustomreportSaveModel : AopObject
    {
        /// <summary>
        /// 自定义报表规则条件信息
        /// </summary>
        [XmlElement("report_condition_info")]
        public CustomReportCondition ReportConditionInfo { get; set; }
    }
}
