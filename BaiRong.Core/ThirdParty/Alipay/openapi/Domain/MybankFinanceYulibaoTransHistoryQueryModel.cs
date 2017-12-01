using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankFinanceYulibaoTransHistoryQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankFinanceYulibaoTransHistoryQueryModel : AopObject
    {
        /// <summary>
        /// 查询交易的结束时间，必须是格式为yyyyMMdd的日期字符串，且日期要大于等于start_date，时间最大跨度为30天，如start_date为20160808，则end_date最大值为20160906。
        /// </summary>
        [XmlElement("end_date")]
        public string EndDate { get; set; }

        /// <summary>
        /// 基金代码，必填。目前默认填001529，代表余利宝。
        /// </summary>
        [XmlElement("fund_code")]
        public string FundCode { get; set; }

        /// <summary>
        /// 页码，历史交易记录分页展示的页码。必须为正整数，最大值为99。
        /// </summary>
        [XmlElement("page")]
        public long Page { get; set; }

        /// <summary>
        /// 每页条数，历史交易记录查询时每页的最大条数。必须为正整数，最大值为50。
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 查询交易的开始时间，必须是格式为yyyyMMdd的日期字符串，如20160808。
        /// </summary>
        [XmlElement("start_date")]
        public string StartDate { get; set; }
    }
}
