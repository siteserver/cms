using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicFollowBatchqueryResponse.
    /// </summary>
    public class AlipayOpenPublicFollowBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 本次调用获取的userId个数，最大值为10000
        /// </summary>
        [XmlElement("count")]
        public string Count { get; set; }

        /// <summary>
        /// 查询分组的userid
        /// </summary>
        [XmlElement("next_user_id")]
        public string NextUserId { get; set; }

        /// <summary>
        /// 用户的userId列表
        /// </summary>
        [XmlArray("user_id_list")]
        [XmlArrayItem("string")]
        public List<string> UserIdList { get; set; }
    }
}
