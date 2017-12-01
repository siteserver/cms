using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankFinanceYulibaoPriceQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankFinanceYulibaoPriceQueryModel : AopObject
    {
        /// <summary>
        /// 查询行情的截止日期，必须是格式为yyyyMMdd的日期字符串且日期要大于等于start_date，时间最大跨度为30天，如start_date为20160808，则end_date最大值为20160906
        /// </summary>
        [XmlElement("end_date")]
        public string EndDate { get; set; }

        /// <summary>
        /// 基金代码，必填。目前默认填001529，代表余利宝。
        /// </summary>
        [XmlElement("fund_code")]
        public string FundCode { get; set; }

        /// <summary>
        /// 查询行情的开始日期，必须是格式为yyyyMMdd的日期字符串，如20160808
        /// </summary>
        [XmlElement("start_date")]
        public string StartDate { get; set; }
    }
}
