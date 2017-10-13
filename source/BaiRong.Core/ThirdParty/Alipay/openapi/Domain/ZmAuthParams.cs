using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZmAuthParams Data Structure.
    /// </summary>
    [Serializable]
    public class ZmAuthParams : AopObject
    {
        /// <summary>
        /// 商户在芝麻端申请的appId
        /// </summary>
        [XmlElement("buckle_app_id")]
        public string BuckleAppId { get; set; }

        /// <summary>
        /// 商户在芝麻端申请的merchantId
        /// </summary>
        [XmlElement("buckle_merchant_id")]
        public string BuckleMerchantId { get; set; }
    }
}
