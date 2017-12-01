using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SubMerchant Data Structure.
    /// </summary>
    [Serializable]
    public class SubMerchant : AopObject
    {
        /// <summary>
        /// 二级商户的支付宝id
        /// </summary>
        [XmlElement("merchant_id")]
        public string MerchantId { get; set; }
    }
}
