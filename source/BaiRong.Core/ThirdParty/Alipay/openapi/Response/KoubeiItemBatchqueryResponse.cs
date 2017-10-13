using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiItemBatchqueryResponse.
    /// </summary>
    public class KoubeiItemBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        [XmlElement("current_page_no")]
        public string CurrentPageNo { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        [XmlArray("item_infos")]
        [XmlArrayItem("item_query_response")]
        public List<ItemQueryResponse> ItemInfos { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }

        /// <summary>
        /// 总共商品数目
        /// </summary>
        [XmlElement("total_items")]
        public string TotalItems { get; set; }

        /// <summary>
        /// 总页码数目
        /// </summary>
        [XmlElement("total_page_no")]
        public string TotalPageNo { get; set; }
    }
}
