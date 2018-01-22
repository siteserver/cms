using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayTradeCustomsQueryResponse.
    /// </summary>
    public class AlipayTradeCustomsQueryResponse : AopResponse
    {
        /// <summary>
        /// 不存在记录的报关请求号。多个值用逗号分隔，单次最多10个;每个报关请求号String(32)
        /// </summary>
        [XmlElement("not_found")]
        public string NotFound { get; set; }

        /// <summary>
        /// 匹配到的列表。每个记录代表一条报关记录
        /// </summary>
        [XmlArray("records")]
        [XmlArrayItem("customs_declare_record_info")]
        public List<CustomsDeclareRecordInfo> Records { get; set; }
    }
}
