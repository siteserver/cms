using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsLiability Data Structure.
    /// </summary>
    [Serializable]
    public class InsLiability : AopObject
    {
        /// <summary>
        /// 责任描述
        /// </summary>
        [XmlElement("liability_desc")]
        public string LiabilityDesc { get; set; }

        /// <summary>
        /// 责任名称
        /// </summary>
        [XmlElement("liability_name")]
        public string LiabilityName { get; set; }

        /// <summary>
        /// 保额
        /// </summary>
        [XmlElement("sum_insured")]
        public InsSumInsured SumInsured { get; set; }
    }
}
