using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlisisReport Data Structure.
    /// </summary>
    [Serializable]
    public class AlisisReport : AopObject
    {
        /// <summary>
        /// ：   报表可过滤字段条件
        /// </summary>
        [XmlArray("conditions")]
        [XmlArrayItem("report_condition")]
        public List<ReportCondition> Conditions { get; set; }

        /// <summary>
        /// 报表描述
        /// </summary>
        [XmlElement("report_desc")]
        public string ReportDesc { get; set; }

        /// <summary>
        /// 报表名称
        /// </summary>
        [XmlElement("report_name")]
        public string ReportName { get; set; }

        /// <summary>
        /// 报表唯一标识
        /// </summary>
        [XmlElement("report_uk")]
        public string ReportUk { get; set; }
    }
}
