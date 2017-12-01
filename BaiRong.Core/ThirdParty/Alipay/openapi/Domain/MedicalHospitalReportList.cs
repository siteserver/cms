using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MedicalHospitalReportList Data Structure.
    /// </summary>
    [Serializable]
    public class MedicalHospitalReportList : AopObject
    {
        /// <summary>
        /// 报告产出日期 格式为yyyy-MM-dd HH:mm:ss。
        /// </summary>
        [XmlElement("report_date")]
        public string ReportDate { get; set; }

        /// <summary>
        /// 报告说明
        /// </summary>
        [XmlElement("report_desc")]
        public string ReportDesc { get; set; }

        /// <summary>
        /// 报告详情连接
        /// </summary>
        [XmlElement("report_link")]
        public string ReportLink { get; set; }

        /// <summary>
        /// 报告名称
        /// </summary>
        [XmlElement("report_name")]
        public string ReportName { get; set; }

        /// <summary>
        /// 报告类型:  CHECK_REPORT 检查报告  EXAM_REPORT检验报告
        /// </summary>
        [XmlElement("report_type")]
        public string ReportType { get; set; }
    }
}
