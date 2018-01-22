using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlisisReportRow Data Structure.
    /// </summary>
    [Serializable]
    public class AlisisReportRow : AopObject
    {
        /// <summary>
        /// 报表行信息，每个对象是一列的数据
        /// </summary>
        [XmlArray("row_data")]
        [XmlArrayItem("alisis_report_column")]
        public List<AlisisReportColumn> RowData { get; set; }
    }
}
