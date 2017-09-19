using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingDataMemberReportQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingDataMemberReportQueryModel : AopObject
    {
        /// <summary>
        /// 查询报表数据的业务日期，精确到天，格式为yyyymmdd，数据按天返回
        /// </summary>
        [XmlElement("biz_date")]
        public string BizDate { get; set; }
    }
}
