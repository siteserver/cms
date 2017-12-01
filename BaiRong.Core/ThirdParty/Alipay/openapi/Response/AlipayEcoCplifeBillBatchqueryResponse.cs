using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoCplifeBillBatchqueryResponse.
    /// </summary>
    public class AlipayEcoCplifeBillBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 若查询到符合条件的账单条目，返回结果集
        /// </summary>
        [XmlArray("bill_result_set")]
        [XmlArrayItem("c_p_bill_result_set")]
        public List<CPBillResultSet> BillResultSet { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        [XmlElement("current_page_num")]
        public long CurrentPageNum { get; set; }

        /// <summary>
        /// 符合条件的总结果数
        /// </summary>
        [XmlElement("total_bill_count")]
        public long TotalBillCount { get; set; }
    }
}
