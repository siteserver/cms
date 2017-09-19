using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayZdataserviceUnidataQueryResponse.
    /// </summary>
    public class AlipayZdataserviceUnidataQueryResponse : AopResponse
    {
        /// <summary>
        /// 查询结果集，统一用string方式返回
        /// </summary>
        [XmlElement("query_result")]
        public string QueryResult { get; set; }

        /// <summary>
        /// 查询是否成功具体描述
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 查询是否成功
        /// </summary>
        [XmlElement("success")]
        public string Success { get; set; }
    }
}
