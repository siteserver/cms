using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceTransportOfflinepayUserblacklistQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceTransportOfflinepayUserblacklistQueryModel : AopObject
    {
        /// <summary>
        /// 用户黑名单分页ID，1开始
        /// </summary>
        [XmlElement("page_index")]
        public long PageIndex { get; set; }

        /// <summary>
        /// 脱机交易用户黑名单分页页大小，最大页大小不超过1000
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }
    }
}
