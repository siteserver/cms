using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionAdvchannelBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionAdvchannelBatchqueryModel : AopObject
    {
        /// <summary>
        /// 广告ID
        /// </summary>
        [XmlElement("adv_id")]
        public string AdvId { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        [XmlElement("page_index")]
        public string PageIndex { get; set; }

        /// <summary>
        /// 每页记录数，默认10，最大100
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }
    }
}
