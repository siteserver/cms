using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionChannelBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionChannelBatchqueryModel : AopObject
    {
        /// <summary>
        /// 页码
        /// </summary>
        [XmlElement("page_index")]
        public string PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }
    }
}
