using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingVoucherListQueryResponse.
    /// </summary>
    public class AlipayMarketingVoucherListQueryResponse : AopResponse
    {
        /// <summary>
        /// 券列表，一定不为null，但是size可以为0
        /// </summary>
        [XmlArray("vouchers")]
        [XmlArrayItem("voucher_lite_info")]
        public List<VoucherLiteInfo> Vouchers { get; set; }
    }
}
