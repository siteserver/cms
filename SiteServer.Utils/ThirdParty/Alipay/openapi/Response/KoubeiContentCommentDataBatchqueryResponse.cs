using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiContentCommentDataBatchqueryResponse.
    /// </summary>
    public class KoubeiContentCommentDataBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 评价结果列表
        /// </summary>
        [XmlArray("comments")]
        [XmlArrayItem("comment_open_model")]
        public List<CommentOpenModel> Comments { get; set; }

        /// <summary>
        /// 当前查询条件下评价总数
        /// </summary>
        [XmlElement("count")]
        public long Count { get; set; }
    }
}
