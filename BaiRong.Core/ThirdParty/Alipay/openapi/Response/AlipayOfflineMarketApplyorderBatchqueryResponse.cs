using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketApplyorderBatchqueryResponse.
    /// </summary>
    public class AlipayOfflineMarketApplyorderBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 支付宝操作流水信息列表
        /// </summary>
        [XmlArray("biz_order_infos")]
        [XmlArrayItem("biz_order_query_response")]
        public List<BizOrderQueryResponse> BizOrderInfos { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        [XmlElement("current_page_no")]
        public long CurrentPageNo { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [XmlElement("total_items")]
        public long TotalItems { get; set; }

        /// <summary>
        /// 总页码数目
        /// </summary>
        [XmlElement("total_page_no")]
        public long TotalPageNo { get; set; }
    }
}
