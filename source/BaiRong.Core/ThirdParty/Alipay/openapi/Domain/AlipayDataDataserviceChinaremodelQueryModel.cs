using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataDataserviceChinaremodelQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataDataserviceChinaremodelQueryModel : AopObject
    {
        /// <summary>
        /// 体检记录id
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 规则id
        /// </summary>
        [XmlElement("rule_id")]
        public string RuleId { get; set; }

        /// <summary>
        /// 交易流水记录id
        /// </summary>
        [XmlElement("trans_id")]
        public string TransId { get; set; }
    }
}
