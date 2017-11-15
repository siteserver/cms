using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InstallmentMetaInfo Data Structure.
    /// </summary>
    [Serializable]
    public class InstallmentMetaInfo : AopObject
    {
        /// <summary>
        /// 结束期数，包含此值
        /// </summary>
        [XmlElement("end_term")]
        public long EndTerm { get; set; }

        /// <summary>
        /// 开始期数，包含此值
        /// </summary>
        [XmlElement("start_term")]
        public long StartTerm { get; set; }

        /// <summary>
        /// 分期值（如还款方式、利率等）
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
