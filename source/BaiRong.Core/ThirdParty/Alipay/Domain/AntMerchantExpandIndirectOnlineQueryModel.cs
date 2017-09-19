using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntMerchantExpandIndirectOnlineQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntMerchantExpandIndirectOnlineQueryModel : AopObject
    {
        /// <summary>
        /// 受理商户在受理机构下的唯一标识，与sub_merchant_id二选一必传
        /// </summary>
        [XmlElement("external_id")]
        public string ExternalId { get; set; }

        /// <summary>
        /// 商户在支付宝入驻成功后，生成的支付宝内全局唯一的商户编号，与external_id二选一必传
        /// </summary>
        [XmlElement("sub_merchant_id")]
        public string SubMerchantId { get; set; }
    }
}
