using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// MybankFinanceYulibaoTransHistoryQueryResponse.
    /// </summary>
    public class MybankFinanceYulibaoTransHistoryQueryResponse : AopResponse
    {
        /// <summary>
        /// 历史交易记录查询的当前页码
        /// </summary>
        [XmlElement("current_page")]
        public long CurrentPage { get; set; }

        /// <summary>
        /// 当前查询是否具有下一页的数据，true-有，fasle-没有
        /// </summary>
        [XmlElement("has_next_page")]
        public bool HasNextPage { get; set; }

        /// <summary>
        /// 历史交易详情信息
        /// </summary>
        [XmlArray("history_trans_detail_infos")]
        [XmlArrayItem("y_l_b_trans_detail_info")]
        public List<YLBTransDetailInfo> HistoryTransDetailInfos { get; set; }

        /// <summary>
        /// 当前查询在不分页情况下的数据总数
        /// </summary>
        [XmlElement("total_item_count")]
        public string TotalItemCount { get; set; }
    }
}
