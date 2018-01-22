using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflinePayMasterKey Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflinePayMasterKey : AopObject
    {
        /// <summary>
        /// 秘钥id
        /// </summary>
        [XmlElement("key_id")]
        public long KeyId { get; set; }

        /// <summary>
        /// 支付宝脱机服务公钥
        /// </summary>
        [XmlElement("public_key")]
        public string PublicKey { get; set; }
    }
}
