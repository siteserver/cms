using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcapiprodDrawndnRepaymentrecordQueryResponse.
    /// </summary>
    public class AlipayEcapiprodDrawndnRepaymentrecordQueryResponse : AopResponse
    {
        /// <summary>
        /// 还款流水
        /// </summary>
        [XmlArray("repayment_records")]
        [XmlArrayItem("repayment_records")]
        public List<RepaymentRecords> RepaymentRecords { get; set; }

        /// <summary>
        /// 唯一标识这次请求
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}
