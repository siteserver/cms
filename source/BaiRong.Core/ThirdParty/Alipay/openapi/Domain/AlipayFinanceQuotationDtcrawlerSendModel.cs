using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFinanceQuotationDtcrawlerSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFinanceQuotationDtcrawlerSendModel : AopObject
    {
        /// <summary>
        /// 爬虫平台推送数据，为json字符串，bizNo 为推送流水号，taskName为任务名，业务数据包含在bizData中
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }
    }
}
