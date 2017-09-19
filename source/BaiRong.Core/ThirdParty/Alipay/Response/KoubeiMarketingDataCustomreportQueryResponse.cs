using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataCustomreportQueryResponse.
    /// </summary>
    public class KoubeiMarketingDataCustomreportQueryResponse : AopResponse
    {
        /// <summary>
        /// 数据量
        /// </summary>
        [XmlElement("count")]
        public string Count { get; set; }

        /// <summary>
        /// 满足自定义报表规则的报表数据
        /// </summary>
        [XmlArray("report_data")]
        [XmlArrayItem("report_data_item")]
        public List<ReportDataItem> ReportData { get; set; }
    }
}
