using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketingVoucherCreateResponse.
    /// </summary>
    public class AlipayOfflineMarketingVoucherCreateResponse : AopResponse
    {
        /// <summary>
        /// 活动id，过渡方案的兼容字段
        /// </summary>
        [XmlElement("activity_id")]
        public string ActivityId { get; set; }

        /// <summary>
        /// 券模板id
        /// </summary>
        [XmlElement("voucher_id")]
        public string VoucherId { get; set; }
    }
}
