using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCdpAdvertiseReportQueryResponse.
    /// </summary>
    public class AlipayMarketingCdpAdvertiseReportQueryResponse : AopResponse
    {
        /// <summary>
        /// 日点击量
        /// </summary>
        [XmlElement("click_pv_dd")]
        public string ClickPvDd { get; set; }

        /// <summary>
        /// 历史累积点击量
        /// </summary>
        [XmlElement("click_pv_history")]
        public string ClickPvHistory { get; set; }

        /// <summary>
        /// 日点击用户数
        /// </summary>
        [XmlElement("click_uv_dd")]
        public string ClickUvDd { get; set; }

        /// <summary>
        /// 历史累积点击用户数
        /// </summary>
        [XmlElement("click_uv_history")]
        public string ClickUvHistory { get; set; }

        /// <summary>
        /// 效果分析日期
        /// </summary>
        [XmlElement("report_date")]
        public string ReportDate { get; set; }

        /// <summary>
        /// 日曝光量
        /// </summary>
        [XmlElement("show_pv_dd")]
        public string ShowPvDd { get; set; }

        /// <summary>
        /// 历史累积曝光量
        /// </summary>
        [XmlElement("show_pv_history")]
        public string ShowPvHistory { get; set; }

        /// <summary>
        /// 日曝光用户数
        /// </summary>
        [XmlElement("show_uv_dd")]
        public string ShowUvDd { get; set; }

        /// <summary>
        /// 历史累积曝光用户数
        /// </summary>
        [XmlElement("show_uv_history")]
        public string ShowUvHistory { get; set; }
    }
}
