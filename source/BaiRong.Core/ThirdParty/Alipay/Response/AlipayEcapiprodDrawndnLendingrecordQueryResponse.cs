using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcapiprodDrawndnLendingrecordQueryResponse.
    /// </summary>
    public class AlipayEcapiprodDrawndnLendingrecordQueryResponse : AopResponse
    {
        /// <summary>
        /// 放款流水
        /// </summary>
        [XmlArray("lending_records")]
        [XmlArrayItem("lending_records")]
        public List<LendingRecords> LendingRecords { get; set; }

        /// <summary>
        /// 代表一次请求的唯一编号，用于追溯问题，多方联调查询问题时，ISV 可以提供该RequestId给网关，网关用来查询本次请求的具体日志
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}
