using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicGroupBatchqueryResponse.
    /// </summary>
    public class AlipayOpenPublicGroupBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 用户分组列表，包含每个分组的id、name、以及规则模型
        /// </summary>
        [XmlArray("groups")]
        [XmlArrayItem("query_group")]
        public List<QueryGroup> Groups { get; set; }
    }
}
