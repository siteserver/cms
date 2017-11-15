using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiCraftsmanDataWorkBatchqueryResponse.
    /// </summary>
    public class KoubeiCraftsmanDataWorkBatchqueryResponse : AopResponse
    {
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
        /// 总页码数目
        /// </summary>
        [XmlElement("total_page_no")]
        public long TotalPageNo { get; set; }

        /// <summary>
        /// 总共手艺人作品数目
        /// </summary>
        [XmlElement("total_works")]
        public long TotalWorks { get; set; }

        /// <summary>
        /// 作品信息列表
        /// </summary>
        [XmlArray("works")]
        [XmlArrayItem("craftsman_work_open_model")]
        public List<CraftsmanWorkOpenModel> Works { get; set; }
    }
}
