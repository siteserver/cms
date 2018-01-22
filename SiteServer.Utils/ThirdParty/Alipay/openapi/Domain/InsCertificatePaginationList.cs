using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsCertificatePaginationList Data Structure.
    /// </summary>
    [Serializable]
    public class InsCertificatePaginationList : AopObject
    {
        /// <summary>
        /// 当前页数
        /// </summary>
        [XmlElement("current_page")]
        public long CurrentPage { get; set; }

        /// <summary>
        /// 结果列表
        /// </summary>
        [XmlArray("list")]
        [XmlArrayItem("ins_certificate_api_d_t_o")]
        public List<InsCertificateApiDTO> List { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [XmlElement("total_count")]
        public long TotalCount { get; set; }

        /// <summary>
        /// 全部页数
        /// </summary>
        [XmlElement("total_page_num")]
        public long TotalPageNum { get; set; }
    }
}
