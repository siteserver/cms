using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingVoucherTemplatelistQueryResponse.
    /// </summary>
    public class AlipayMarketingVoucherTemplatelistQueryResponse : AopResponse
    {
        /// <summary>
        /// 当前页码,页码从1开始
        /// </summary>
        [XmlElement("current_page")]
        public long CurrentPage { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        [XmlElement("items_per_page")]
        public long ItemsPerPage { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        [XmlElement("total_items")]
        public long TotalItems { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [XmlElement("total_pages")]
        public long TotalPages { get; set; }

        /// <summary>
        /// 券模板列表
        /// </summary>
        [XmlArray("voucher_templates")]
        [XmlArrayItem("voucher_template_lite_info")]
        public List<VoucherTemplateLiteInfo> VoucherTemplates { get; set; }
    }
}
